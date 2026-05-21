using Generic.Application.Dtos;
using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Helpers;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Catalog.Enums;
using Ingressinhos.Domain.Sales.Entities;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.UseCases;

public class CloseOrder : IUseCaseCloseOrder
{
    private readonly IRepositorySession _repositorySession;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IRequestPayment _requestPayment;

    public CloseOrder(IRepositorySession repositorySession, ICurrentUserContext currentUserContext, IRequestPayment requestPayment)
    {
        _repositorySession = repositorySession;
        _currentUserContext = currentUserContext;
        _requestPayment = requestPayment;
    }

    public OperationResult<PaymentCheckoutApiDto> Execute(long orderId)
    {
        if (orderId <= 0)
        {
            return OperationResult<PaymentCheckoutApiDto>.UnprocessableEntity(new MensagemErro("Pedido", "Deve ser informado o identificador do pedido."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var order = repositoryQuery.Return<OrderDomain>(orderId);
            if (order is null)
            {
                return OperationResult<PaymentCheckoutApiDto>.NotFound(new MensagemErro("Pedido", "Pedido nao encontrado."));
            }

            var accessResult = EnsureOrderAccess(order, repositoryQuery);
            if (!accessResult.Success)
            {
                return OperationResult<PaymentCheckoutApiDto>.FromResult(accessResult);
            }

            var repository = _repositorySession.GetRepository();
            using var transaction = _repositorySession.BeginTransaction();
            var utcNow = DateTime.UtcNow;

            order.MoveToPendingPayment();
            if (!order.IsValid)
            {
                _repositorySession.RollbackTransaction();
                return OperationResult<PaymentCheckoutApiDto>.FromResult(order.ToUnprocessableEntityResult());
            }

            order.UpdatedAt = utcNow;

            var reserveResult = ReserveOrderInventory(order, repositoryQuery, repository, utcNow);
            if (!reserveResult.Success)
            {
                _repositorySession.RollbackTransaction();
                return OperationResult<PaymentCheckoutApiDto>.FromResult(reserveResult);
            }

            repository.Upsert(order);
            repository.Flush().GetAwaiter().GetResult();

            var paymentResult = RequestPayment(order);
            if (!paymentResult.Success)
            {
                _repositorySession.RollbackTransaction();
                return OperationResult<PaymentCheckoutApiDto>.FromResult(paymentResult);
            }

            _repositorySession.CommitTransaction();

            return paymentResult;
        }
        catch (Exception ex)
        {
            _repositorySession.RollbackTransaction();
            return OperationResult<PaymentCheckoutApiDto>.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }

    internal OperationResult EnsureOrderAccess(OrderDomain order, IRepositoryQuery repositoryQuery)
    {
        if (_currentUserContext.Role == "Admin")
        {
            return OperationResult.Ok();
        }

        var client = CurrentUserEntityResolver.ResolveClient(_currentUserContext, repositoryQuery);
        if (client is null)
        {
            return OperationResult.Unauthorized(new MensagemErro("Perfil", "Nao foi possivel localizar o perfil da sua conta."));
        }

        if (order.ClientId != client.Id)
        {
            return OperationResult.Forbidden(new MensagemErro("Pedido", "Voce so pode fechar pedidos da sua propria conta."));
        }

        return OperationResult.Ok();
    }

    internal static OperationResult ReserveOrderInventory(OrderDomain order, IRepositoryQuery repositoryQuery, IRepository repository, DateTime utcNow)
    {
        foreach (var item in order.Items)
        {
            var ticket = repositoryQuery.Return<Ticket>(item.TicketId);
            if (ticket is null)
            {
                return OperationResult.NotFound(new MensagemErro("Ingresso", $"Nao foi possivel localizar o ingresso do item {item.Id}."));
            }

            ticket.Reserve(item.Quantity, utcNow);
            if (!ticket.IsValid)
            {
                return ticket.ToUnprocessableEntityResult();
            }

            repository.Upsert(ticket);

            if (!item.SeatId.HasValue)
            {
                continue;
            }

            var seat = repositoryQuery.Return<Seat>(item.SeatId.Value);
            if (seat is null)
            {
                return OperationResult.NotFound(new MensagemErro("SeatId", $"Nao foi possivel localizar o assento do item {item.Id}."));
            }

            if (seat.Status == SeatStatus.Reserved || seat.Status == SeatStatus.Occupied || seat.Status == SeatStatus.Blocked)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("SeatId", $"O assento {seat.Code} nao esta disponivel para reserva."));
            }

            seat.Reserve();
            if (!seat.IsValid)
            {
                return seat.ToUnprocessableEntityResult();
            }

            repository.Upsert(seat);
        }

        return OperationResult.Ok();
    }

    internal OperationResult<PaymentCheckoutApiDto> RequestPayment(OrderDomain order)
    {
        return _requestPayment.CreatePayment(order.Id, order.TotalAmount, "pix").GetAwaiter().GetResult();
    }
}
