namespace Ingressinhos.Application.Sales.Dtos;

public class CreateOrderRequest
{
    public long ClientId { get; set; }
    public List<OrderItemRequest> Items { get; set; } = [];
}
