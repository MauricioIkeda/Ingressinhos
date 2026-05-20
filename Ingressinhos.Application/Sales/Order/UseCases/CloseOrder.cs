using Generic.Application.Dtos;
using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Sales.Entities;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.UseCases;

public class CloseOrder : IUseCaseCloseOrder
{
    private readonly IRepositorySession _repositorySession;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IRequestPayment _requestPayment;

    public CloseOrder( IRepositorySession repositorySession, ICurrentUserContext currentUserContext, IRequestPayment requestPayment)
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

            if (_currentUserContext.Role != "Admin")
            {
                var client = repositoryQuery.Query<Client>(c => c.UserId == _currentUserContext.UserId && c.Active).FirstOrDefault();
                if (client is null)
                {
                    return OperationResult<PaymentCheckoutApiDto>.Unauthorized(new MensagemErro("Perfil", "Nao foi possivel localizar o perfil da sua conta."));
                }

                if (order.ClientId != client.Id)
                {
                    return OperationResult<PaymentCheckoutApiDto>.Forbidden(new MensagemErro("Pedido", "Voce so pode fechar pedidos da sua propria conta."));
                }
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

            var reserveResult = ReserveOrderTickets(order, repositoryQuery, repository, utcNow);
            if (!reserveResult.Success)
            {
                _repositorySession.RollbackTransaction();
                return OperationResult<PaymentCheckoutApiDto>.FromResult(reserveResult);
            }

            repository.Upsert(order);
            repository.Flush().GetAwaiter().GetResult();

            var paymentResult = _requestPayment.CreatePayment(order.Id, order.TotalAmount, "pix").GetAwaiter().GetResult(); // "pix" como padrao
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

// No momento fiz um "helper privado" aqui, talvez a gente utilize mensageria preciso ver se mantenho, ou faço uma forma mais clear
    private static OperationResult ReserveOrderTickets( OrderDomain order, IRepositoryQuery repositoryQuery, IRepository repository, DateTime utcNow)
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
        }

        return OperationResult.Ok();
    }
}
