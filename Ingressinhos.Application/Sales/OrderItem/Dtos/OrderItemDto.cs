namespace Ingressinhos.Application.Sales.Dtos;

public class OrderItemDto
{
    public long OrderItemId { get; set; }
    public long OrderId { get; set; }
    public long TicketId { get; set; }
    public string TicketName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
