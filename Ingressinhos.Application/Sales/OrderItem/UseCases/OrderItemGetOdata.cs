using Generic.Application.Crud.UseCases;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using OrderItemDomain = Ingressinhos.Domain.Sales.Entities.OrderItem;

namespace Ingressinhos.Application.Sales.UseCases;

public class OrderItemGetOdata : UseCaseGetQueryItems<OrderItemDomain, OrderItemQueryItem>
{
    public OrderItemGetOdata(IRepositoryQuery repositoryQuery)
        : base(repositoryQuery)
    {
    }

    protected override OrderItemQueryItem ToQueryItem(OrderItemDomain orderItem)
    {
        return new OrderItemQueryItem
        {
            Id = orderItem.Id,
            OrderId = orderItem.OrderId,
            TicketId = orderItem.TicketId,
            TicketName = orderItem.TicketName,
            Quantity = orderItem.Quantity,
            UnitPrice = orderItem.UnitPrice.Value,
            Category = orderItem.Category,
            TotalPrice = orderItem.TotalPrice,
            CreatedAt = orderItem.CreatedAt,
            UpdatedAt = orderItem.UpdatedAt
        };
    }
}
