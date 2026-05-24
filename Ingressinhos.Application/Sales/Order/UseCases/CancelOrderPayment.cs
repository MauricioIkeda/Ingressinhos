using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Helpers;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Sales.Enums;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.UseCases;

public class CancelOrderPayment : IUseCaseCancelOrderPayment
{
    private readonly IRepositorySession _repositorySession;

    public CancelOrderPayment(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult Execute(long orderId)
    {
        if (orderId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Pedido", "Deve ser informado o identificador do pedido."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var order = repositoryQuery.Return<OrderDomain>(orderId);

            if (order is null)
            {
                return OperationResult.NotFound(new MensagemErro("Pedido", "Pedido nao encontrado."));
            }

            // Idempotencia: se a mensagem chegar de novo, nao quebramos o fluxo.
            if (order.Status == OrderStatus.Cancelled)
            {
                return OperationResult.Ok();
            }

            if (order.Status == OrderStatus.Paid)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("Pedido", "Nao e possivel cancelar um pedido ja pago."));
            }

            if (order.Status != OrderStatus.PendingPayment)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("Pedido", "Somente pedidos pendentes de pagamento podem devolver reserva."));
            }

            var repository = _repositorySession.GetRepository();
            using var transaction = _repositorySession.BeginTransaction();

            var restoreResult = RestoreReservedTickets(order, repositoryQuery, repository);
            if (!restoreResult.Success)
            {
                _repositorySession.RollbackTransaction();
                return restoreResult;
            }

            order.Cancel();
            if (!order.IsValid)
            {
                _repositorySession.RollbackTransaction();
                return order.ToUnprocessableEntityResult();
            }

            order.UpdatedAt = DateTime.UtcNow;

            repository.Upsert(order);
            repository.Flush().GetAwaiter().GetResult();
            _repositorySession.CommitTransaction();

            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            _repositorySession.RollbackTransaction();
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }

    // No momento fiz um "helper privado" aqui, talvez a gente utilize mensageria preciso ver se mantenho, ou faço uma forma mais clear
    private static OperationResult RestoreReservedTickets(OrderDomain order, IRepositoryQuery repositoryQuery, IRepository repository)
    {
        foreach (var item in order.Items)
        {
            var ticket = repositoryQuery.Return<Ticket>(item.TicketId);
            if (ticket is null)
            {
                return OperationResult.NotFound(new MensagemErro("Ingresso", $"Nao foi possivel localizar o ingresso do item {item.Id}."));
            }

            ticket.RestoreQuantity(item.Quantity);
            if (!ticket.IsValid)
            {
                return ticket.ToUnprocessableEntityResult();
            }

            repository.Upsert(ticket);

            if (!item.SeatId.HasValue)
            {
                continue;
            }

            var seatReservation = SeatReservationRulesHelper.GetActiveReservationForOrderItem(repositoryQuery, order.Id, item.Id);
            if (seatReservation is null)
            {
                continue;
            }

            seatReservation.Cancel();
            if (!seatReservation.IsValid)
            {
                return seatReservation.ToUnprocessableEntityResult();
            }

            seatReservation.UpdatedAt = DateTime.UtcNow;
            repository.Upsert(seatReservation);
        }

        return OperationResult.Ok();
    }
}
