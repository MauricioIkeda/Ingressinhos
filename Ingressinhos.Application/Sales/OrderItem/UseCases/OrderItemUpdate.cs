using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using OrderItemDomain = Ingressinhos.Domain.Sales.Entities.OrderItem;

namespace Ingressinhos.Application.Sales.UseCases;

public class OrderItemUpdate : IUseCaseCommand<OrderItemDto>
{
    public ListMessages Messages { get; } = new();

    private readonly IRepositorySession _repositorySession;

    public OrderItemUpdate(IRepositorySession repositorySession)
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

        if (orderItemDto.OrderItemId <= 0)
        {
            Messages.Add("Deve ser informado o identificador do item do pedido", error: true);
            return false;
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var orderItemEntity = repositoryQuery.Return<OrderItemDomain>(orderItemDto.OrderItemId);

            if (orderItemEntity is null)
            {
                Messages.Add("Item do pedido nao encontrado", error: true);
                return false;
            }

            if (orderItemDto.OrderId != orderItemEntity.OrderId ||
                orderItemDto.TicketId != orderItemEntity.TicketId ||
                orderItemDto.TicketName != orderItemEntity.TicketName ||
                orderItemDto.Quantity != orderItemEntity.Quantity ||
                orderItemDto.UnitPrice != orderItemEntity.UnitPrice.Value)
            {
                Messages.Add("Nao eh permitido alterar um item de pedido existente. Exclua e inclua novamente.", error: true);
                return false;
            }

            orderItemEntity.UpdatedAt = DateTime.UtcNow;

            var repository = _repositorySession.GetRepository();
            repository.Upsert(orderItemEntity);
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
