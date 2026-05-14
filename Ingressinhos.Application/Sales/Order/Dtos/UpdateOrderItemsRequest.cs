namespace Ingressinhos.Application.Sales.Dtos;

public class UpdateOrderItemsRequest
{
    public long OrderId { get; set; }
    public List<OrderItemRequest> Items { get; set; } = [];
}
