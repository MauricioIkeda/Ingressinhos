using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Helpers;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Domain.Sales.Enums;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.UseCases;

public class ConfirmOrderPayment : IUseCaseConfirmOrderPayment
{
    private readonly IRepositorySession _repositorySession;

    public ConfirmOrderPayment(IRepositorySession repositorySession)
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
            if (order.Status == OrderStatus.Paid)
            {
                return OperationResult.Ok();
            }

            order.ConfirmPayment();
            if (!order.IsValid)
            {
                return order.ToUnprocessableEntityResult();
            }

            order.UpdatedAt = DateTime.UtcNow;

            var repository = _repositorySession.GetRepository();
            foreach (var item in order.Items.Where(item => item.SeatId.HasValue))
            {
                var seatReservation = SeatReservationRulesHelper.GetActiveReservationForOrderItem(repositoryQuery, order.Id, item.Id);
                if (seatReservation is null)
                {
                    return OperationResult.NotFound(new MensagemErro("SeatId", $"Reserva de assento do item {item.Id} nao encontrada."));
                }

                seatReservation.Occupy();
                if (!seatReservation.IsValid)
                {
                    return seatReservation.ToUnprocessableEntityResult();
                }

                seatReservation.UpdatedAt = DateTime.UtcNow;
                repository.Upsert(seatReservation);
            }

            repository.Upsert(order);
            repository.Flush();

            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
