using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using OrderItemDomain = Ingressinhos.Domain.Sales.Entities.OrderItem;

namespace Ingressinhos.Application.Sales.UseCases;

public class OrderItemUpdate
{
    private readonly IRepositorySession _repositorySession;

    public OrderItemUpdate(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(OrderItemDto orderItemDto)
    {
        if (orderItemDto is null)
        {
            throw new Exception("Deve ser informado o item do pedido");
        }

        if (orderItemDto.OrderItemId <= 0)
        {
            throw new Exception("Deve ser informado o identificador do item do pedido");
        }

        var repositoryQuery = _repositorySession.GetRepositoryQuery();
        var orderItemEntity = repositoryQuery.Return<OrderItemDomain>(orderItemDto.OrderItemId);

        if (orderItemEntity is null)
        {
            throw new Exception("Item do pedido nao encontrado");
        }

        if (orderItemDto.OrderId != orderItemEntity.OrderId ||
            orderItemDto.TicketId != orderItemEntity.TicketId ||
            orderItemDto.TicketName != orderItemEntity.TicketName ||
            orderItemDto.Quantity != orderItemEntity.Quantity ||
            orderItemDto.UnitPrice != orderItemEntity.UnitPrice.Value)
        {
            throw new Exception("Nao eh permitido alterar um item de pedido existente. Exclua e inclua novamente.");
        }

        orderItemEntity.UpdatedAt = DateTime.UtcNow;

        var repository = _repositorySession.GetRepository();
        repository.Upsert(orderItemEntity);
        repository.Flush().GetAwaiter().GetResult();
        return true;
    }
}
