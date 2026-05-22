using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Helpers;
using Ingressinhos.Domain.Sales.Entities;
using Ingressinhos.Domain.Sales.Enums;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.UseCases;

public class RemoveCartItem
{
    private readonly IRepositorySession _repositorySession;
    private readonly ICurrentUserContext _currentUserContext;

    public RemoveCartItem(IRepositorySession repositorySession, ICurrentUserContext currentUserContext)
    {
        _repositorySession = repositorySession;
        _currentUserContext = currentUserContext;
    }

    public OperationResult Execute(long orderItemId)
    {
        if (orderItemId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Item", "Deve ser informado o identificador do item do carrinho."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            OrderDomain? order;
            OrderItem? orderItem;

            if (_currentUserContext.Role == "Admin")
            {
                orderItem = repositoryQuery.Return<OrderItem>(orderItemId);
                if (orderItem is null)
                {
                    return OperationResult.NotFound(new MensagemErro("Item", "Item do carrinho nao encontrado."));
                }

                order = repositoryQuery.Return<OrderDomain>(orderItem.OrderId);
                if (order is null)
                {
                    return OperationResult.NotFound(new MensagemErro("Pedido", "Pedido do item nao encontrado."));
                }
            }
            else
            {
                var client = CurrentUserEntityResolver.ResolveClient(_currentUserContext, repositoryQuery);
                if (client is null)
                {
                    return OperationResult.Unauthorized(new MensagemErro("Perfil", "Nao foi possivel localizar o perfil da sua conta."));
                }

                order = repositoryQuery.Query<OrderDomain>(currentOrder => currentOrder.ClientId == client.Id && currentOrder.Status == OrderStatus.Cart)
                    .OrderByDescending(currentOrder => currentOrder.Id).FirstOrDefault(); // pegando o carrinho do desgraçado, só um carrinho por vez

                if (order is null)
                {
                    return OperationResult.NotFound(new MensagemErro("Pedido", "Carrinho nao encontrado."));
                }

                orderItem = order.Items.FirstOrDefault(item => item.Id == orderItemId); //Vê se o carrinho tem o item
                if (orderItem is null)
                {
                    return OperationResult.Forbidden(new MensagemErro("Item", "Este item nao pertence ao seu carrinho atual."));
                }
            }

            var repository = _repositorySession.GetRepository();
            using var transaction = _repositorySession.BeginTransaction();
            var utcNow = DateTime.UtcNow;

            order.RemoveItem(orderItem.TotalPrice);
            if (!order.IsValid)
            {
                _repositorySession.RollbackTransaction();
                return order.ToUnprocessableEntityResult();
            }

            order.UpdatedAt = utcNow;
            repository.Delete(orderItem);
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
}
