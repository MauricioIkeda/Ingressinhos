using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Helpers;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Domain.Sales.Enums;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.UseCases;

public class AddCartItem
{
    private readonly IRepositorySession _repositorySession;
    private readonly ICurrentUserContext _currentUserContext;

    public AddCartItem(IRepositorySession repositorySession, ICurrentUserContext currentUserContext)
    {
        _repositorySession = repositorySession;
        _currentUserContext = currentUserContext;
    }

    public OperationResult Execute(AddCartItemRequest command)
    {
        if (command is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Item", "Envie os dados do item do carrinho."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var client = CurrentUserEntityResolver.ResolveClient(_currentUserContext, repositoryQuery, command.ClientId);
            if (client is null)
            {
                return _currentUserContext.Role == "Admin"
                    ? OperationResult.NotFound(new MensagemErro("Cliente", "Cliente informado nao foi encontrado."))
                    : OperationResult.Unauthorized(new MensagemErro("Perfil", "Nao foi possivel localizar o perfil da sua conta."));
            }

            var repository = _repositorySession.GetRepository();
            using var transaction = _repositorySession.BeginTransaction();
            var utcNow = DateTime.UtcNow;

            var order = repositoryQuery.Query<OrderDomain>(order => order.ClientId == client.Id && order.Status == OrderStatus.Cart)
                .OrderByDescending(order => order.Id)
                .FirstOrDefault();

            if (order is null)
            {
                order = new OrderDomain(client.Id) // não tem carrinho, agora vai ter
                {
                    CreatedAt = utcNow,
                    UpdatedAt = utcNow
                };

                if (!order.IsValid)
                {
                    _repositorySession.RollbackTransaction();
                    return order.ToUnprocessableEntityResult();
                }

                repository.Include(order);
                repository.Flush().GetAwaiter().GetResult();
            }

            var itemRequest = new OrderItemRequest
            {
                TicketId = command.TicketId,
                Quantity = command.Quantity,
                SeatId = command.SeatId
            };

            var buildResult = CartItemRulesHelper.CreateOrderItemFromRequest(order.Id, itemRequest, repositoryQuery, utcNow, null);
            if (!buildResult.Success)
            {
                _repositorySession.RollbackTransaction();
                return CartItemRulesHelper.ConvertItemResult(buildResult);
            }

            var orderItem = buildResult.Data;
            if (orderItem.SeatId.HasValue && order.Items.Any(item => item.SeatId == orderItem.SeatId)) // Se tem assento é item unico
            {
                _repositorySession.RollbackTransaction();
                return OperationResult.UnprocessableEntity(new MensagemErro("SeatId", "Este assento ja esta no carrinho."));
            }

            if (!orderItem.SeatId.HasValue) // Se não tem assento, aumenta a quantidade do item, caso tenha no carrinho
            {
                var existingItem = order.Items.FirstOrDefault(item =>
                    !item.SeatId.HasValue &&
                    item.TicketId == orderItem.TicketId &&
                    item.Category == orderItem.Category &&
                    item.UnitPrice.Value == orderItem.UnitPrice.Value);

                if (existingItem is not null) // se não tem no carrinho, adiciona
                {
                    existingItem.AddQuantity(orderItem.Quantity);
                    if (!existingItem.IsValid)
                    {
                        _repositorySession.RollbackTransaction();
                        return existingItem.ToUnprocessableEntityResult();
                    }

                    existingItem.UpdatedAt = utcNow;
                    order.AddItem(existingItem.UnitPrice.Value, orderItem.Quantity);
                    if (!order.IsValid)
                    {
                        _repositorySession.RollbackTransaction();
                        return order.ToUnprocessableEntityResult();
                    }

                    order.UpdatedAt = utcNow;
                    repository.Upsert(existingItem);
                    repository.Upsert(order);
                    repository.Flush().GetAwaiter().GetResult();
                    _repositorySession.CommitTransaction();
                    return OperationResult.Ok();
                }
            }

            order.AddItem(orderItem.UnitPrice.Value, orderItem.Quantity);// valor calcula lá dentro
            if (!order.IsValid) 
            {
                _repositorySession.RollbackTransaction();
                return order.ToUnprocessableEntityResult();
            }

            order.UpdatedAt = utcNow;
            repository.Include(orderItem);
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
