using Generic.Application.Dtos;
using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Domain.Sales.Entities;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.UseCases;

public class CloseOrder : IUseCaseCloseOrder
{
    private readonly IRepositorySession _repositorySession;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IRequestPayment _requestPayment;

    public CloseOrder(
        IRepositorySession repositorySession,
        ICurrentUserContext currentUserContext,
        IRequestPayment requestPayment)
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

            // precisamos validar se todos os itens do carrinho ainda estao disponiveis antes de fechar o pedido.
            // preciso validar estoque/assentos dos itens do pedido antes de seguir para o pagamento.
            order.MoveToPendingPayment();
            if (!order.IsValid)
            {
                return OperationResult<PaymentCheckoutApiDto>.FromResult(order.ToUnprocessableEntityResult());
            }

            order.UpdatedAt = DateTime.UtcNow;

            var repository = _repositorySession.GetRepository();
            using var transaction = _repositorySession.BeginTransaction();

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
}
