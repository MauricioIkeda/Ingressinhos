using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Helpers;
using Ingressinhos.Domain.Sales.Enums;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.UseCases;

public class GetCurrentCart
{
    private readonly IRepositorySession _repositorySession;
    private readonly ICurrentUserContext _currentUserContext;

    public GetCurrentCart(IRepositorySession repositorySession, ICurrentUserContext currentUserContext)
    {
        _repositorySession = repositorySession;
        _currentUserContext = currentUserContext;
    }

    public OperationResult<OrderDomain> Execute(long clientId = 0)
    {
        var repositoryQuery = _repositorySession.GetRepositoryQuery();
        var client = CurrentUserEntityResolver.ResolveClient(_currentUserContext, repositoryQuery, clientId);
        if (client is null)
        {
            return _currentUserContext.Role == "Admin"
                ? OperationResult<OrderDomain>.NotFound(new MensagemErro("Cliente", "Cliente informado nao foi encontrado."))
                : OperationResult<OrderDomain>.Unauthorized(new MensagemErro("Perfil", "Nao foi possivel localizar o perfil da sua conta."));
        }

        var order = repositoryQuery
            .Query<OrderDomain>(currentOrder => currentOrder.ClientId == client.Id && currentOrder.Status == OrderStatus.Cart)
            .OrderByDescending(currentOrder => currentOrder.Id)
            .FirstOrDefault();

        if (order is null)
        {
            return OperationResult<OrderDomain>.NotFound(new MensagemErro("Pedido", "Carrinho nao encontrado."));
        }

        return OperationResult<OrderDomain>.Ok(order);
    }
}
