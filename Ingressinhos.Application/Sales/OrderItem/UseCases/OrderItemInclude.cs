using Generic.Application.Crud.Interface;
using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Domain.Sales.Entities;
using OrderItemDomain = Ingressinhos.Domain.Sales.Entities.OrderItem;

namespace Ingressinhos.Application.Sales.UseCases;

public class OrderItemInclude : IUseCaseCommand<OrderItemDto>
{
    private readonly IRepositorySession _repositorySession;
    private readonly ICurrentUserContext _currentUserContext;

    public OrderItemInclude(IRepositorySession repositorySession, ICurrentUserContext currentUserContext)
    {
        _repositorySession = repositorySession;
        _currentUserContext = currentUserContext;
    }

    public OperationResult Execute(OrderItemDto orderItemDto)
    {
        if (orderItemDto is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Item do pedido", "Envie os dados do item."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var client = repositoryQuery.Query<Client>(c => c.UserId == _currentUserContext.UserId).FirstOrDefault();
            if (client is null)
            {
                return OperationResult.Unauthorized(new MensagemErro("Perfil", "Nao foi possivel localizar o perfil da sua conta."));
            }

            var order = repositoryQuery.Return<Order>(orderItemDto.OrderId);
            if (order is null)
            {
                return OperationResult.NotFound(new MensagemErro("Pedido", "Nao encontramos o pedido informado."));
            }

            if (order.ClientId != client.Id)
            {
                return OperationResult.Forbidden(new MensagemErro("Pedido", "Voce so pode adicionar itens em pedidos da sua conta."));
            }

            var utcNow = DateTime.UtcNow;

            var orderItemEntity = new OrderItemDomain(
                orderItemDto.OrderId,
                orderItemDto.TicketId,
                orderItemDto.TicketName,
                orderItemDto.Quantity,
                orderItemDto.UnitPrice)
            {
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };
            if (!orderItemEntity.IsValid)
            {
                return orderItemEntity.ToUnprocessableEntityResult();
            }

            var repository = _repositorySession.GetRepository();
            repository.Include(orderItemEntity);
            repository.Flush().GetAwaiter().GetResult();
            return OperationResult.Created();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
