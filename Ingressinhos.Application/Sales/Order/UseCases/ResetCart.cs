using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Helpers;
using Ingressinhos.Domain.Sales.Enums;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.UseCases;

public class ResetCart
{
    private readonly IRepositorySession _repositorySession;
    private readonly ICurrentUserContext _currentUserContext;

    public ResetCart(IRepositorySession repositorySession, ICurrentUserContext currentUserContext)
    {
        _repositorySession = repositorySession;
        _currentUserContext = currentUserContext;
    }

    public OperationResult Execute(long clientId = 0)
    {
        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var client = CurrentUserEntityResolver.ResolveClient(_currentUserContext, repositoryQuery, clientId);
            if (client is null)
            {
                return _currentUserContext.Role == "Admin"
                    ? OperationResult.NotFound(new MensagemErro("Cliente", "Cliente informado nao foi encontrado."))
                    : OperationResult.Unauthorized(new MensagemErro("Perfil", "Nao foi possivel localizar o perfil da sua conta."));
            }

            var order = repositoryQuery.Query<OrderDomain>(currentOrder => currentOrder.ClientId == client.Id && currentOrder.Status == OrderStatus.Cart)
                .OrderByDescending(currentOrder => currentOrder.Id)
                .FirstOrDefault();

            if (order is null)
            {
                return OperationResult.NotFound(new MensagemErro("Pedido", "Carrinho nao encontrado."));
            }

            var repository = _repositorySession.GetRepository();
            using var transaction = _repositorySession.BeginTransaction();
            var utcNow = DateTime.UtcNow;
            var existingItems = order.Items.ToList();

            order.ResetItems();
            if (!order.IsValid)
            {
                _repositorySession.RollbackTransaction();
                return order.ToUnprocessableEntityResult();
            }

            foreach (var existingItem in existingItems)
            {
                repository.Delete(existingItem);
            }

            order.UpdatedAt = utcNow;
            repository.Upsert(order);
            repository.Flush();
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
