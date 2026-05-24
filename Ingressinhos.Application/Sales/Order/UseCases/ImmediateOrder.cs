using Generic.Application.Dtos;
using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Helpers;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Domain.Sales.Entities;

namespace Ingressinhos.Application.Sales.UseCases;

public class ImmediateOrder : IUseCaseImmediateOrder
{
    private readonly IRepositorySession _repositorySession;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IRequestPayment _requestPayment;

    public ImmediateOrder(
        IRepositorySession repositorySession,
        ICurrentUserContext currentUserContext,
        IRequestPayment requestPayment)
    {
        _repositorySession = repositorySession;
        _currentUserContext = currentUserContext;
        _requestPayment = requestPayment;
    }

    public OperationResult<PaymentCheckoutApiDto> Execute(CreateOrderRequest command)
    {
        if (command is null)
        {
            return OperationResult<PaymentCheckoutApiDto>.UnprocessableEntity(new MensagemErro("Pedido", "Envie os dados do pedido."));
        }

        if (command.Items is null || command.Items.Count == 0)
        {
            return OperationResult<PaymentCheckoutApiDto>.UnprocessableEntity(new MensagemErro("Itens", "Informe ao menos um item para criar o pedido."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var client = CurrentUserEntityResolver.ResolveClient(_currentUserContext, repositoryQuery, command.ClientId);
            if (client is null)
            {
                return _currentUserContext.Role == "Admin"
                    ? OperationResult<PaymentCheckoutApiDto>.NotFound(new MensagemErro("Cliente", "Cliente informado nao foi encontrado."))
                    : OperationResult<PaymentCheckoutApiDto>.Unauthorized(new MensagemErro("Perfil", "Nao foi possivel localizar o perfil da sua conta."));
            }

            var repository = _repositorySession.GetRepository();
            using var transaction = _repositorySession.BeginTransaction();
            var utcNow = DateTime.UtcNow;

            // Compra imediata sempre cria um pedido novo.
            // Ela nao reaproveita o carrinho atual do cliente.
            var order = new Order(client.Id)
            {
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };

            if (!order.IsValid)
            {
                return OperationResult<PaymentCheckoutApiDto>.FromResult(order.ToUnprocessableEntityResult());
            }

            repository.Include(order);
            repository.Flush().GetAwaiter().GetResult();

            var usedSeatKeys = new HashSet<(long EventId, long SeatId)>();

            foreach (var item in command.Items)
            {
                var buildResult = CartItemRulesHelper.CreateOrderItemFromRequest(order.Id, item, repositoryQuery, utcNow, usedSeatKeys);
                if (!buildResult.Success)
                {
                    _repositorySession.RollbackTransaction();
                    return OperationResult<PaymentCheckoutApiDto>.FromResult(CartItemRulesHelper.ConvertItemResult(buildResult));
                }

                var orderItem = buildResult.Data;
                order.AddItem(orderItem.UnitPrice.Value, orderItem.Quantity);
                if (!order.IsValid)
                {
                    _repositorySession.RollbackTransaction();
                    return OperationResult<PaymentCheckoutApiDto>.FromResult(order.ToUnprocessableEntityResult());
                }

                repository.Include(orderItem);
            }

            order.MoveToPendingPayment();
            if (!order.IsValid)
            {
                _repositorySession.RollbackTransaction();
                return OperationResult<PaymentCheckoutApiDto>.FromResult(order.ToUnprocessableEntityResult());
            }

            order.UpdatedAt = utcNow;
            repository.Upsert(order);
            repository.Flush().GetAwaiter().GetResult();

            // Antes de pedir o pagamento, o pedido ja reserva estoque e assento
            // para evitar que outra compra pegue o mesmo lugar no intervalo.
            var reserveResult = CloseOrder.ReserveOrderInventory(order, repositoryQuery, repository, utcNow);
            if (!reserveResult.Success)
            {
                _repositorySession.RollbackTransaction();
                return OperationResult<PaymentCheckoutApiDto>.FromResult(reserveResult);
            }

            repository.Upsert(order);
            repository.Flush().GetAwaiter().GetResult();

            var paymentResult = _requestPayment.CreatePayment(order.Id, order.TotalAmount, "pix").GetAwaiter().GetResult();
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
            if (SeatReservationRulesHelper.IsSeatReservationConflict(ex))
            {
                return OperationResult<PaymentCheckoutApiDto>.UnprocessableEntity(new MensagemErro("SeatId", "O assento nao esta disponivel para este evento."));
            }

            return OperationResult<PaymentCheckoutApiDto>.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
