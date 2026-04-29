using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using OrderItemDomain = Ingressinhos.Domain.Sales.Entities.OrderItem;

namespace Ingressinhos.Application.Sales.UseCases;

public class OrderItemInclude : IUseCaseCommand<OrderItemDto>
{
    public ListMessages Messages { get; } = new();

    private readonly IRepositorySession _repositorySession;

    public OrderItemInclude(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(OrderItemDto orderItemDto)
    {
        Messages.Clear();

        if (orderItemDto is null)
        {
            Messages.Add("Deve ser informado o item do pedido", error: true);
            return false;
        }

        try
        {
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
        catch (Exception ex)
        {
            Messages.Add(ex);
            return false;
        }
    }
}
