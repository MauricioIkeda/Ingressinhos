using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using OrderItemDomain = Ingressinhos.Domain.Sales.Entities.OrderItem;

namespace Ingressinhos.Application.Sales.UseCases;

public class OrderItemInclude
{
    private readonly IRepositorySession _repositorySession;

    public OrderItemInclude(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(OrderItemDto orderItemDto)
    {
        if (orderItemDto is null)
        {
            throw new Exception("Deve ser informado o item do pedido");
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

        var repository = _repositorySession.GetRepository();
        repository.Include(orderItemEntity);
        repository.Flush().GetAwaiter().GetResult();
        return true;
    }
}
