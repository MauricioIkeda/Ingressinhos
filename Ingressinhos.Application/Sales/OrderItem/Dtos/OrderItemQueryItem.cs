using Ingressinhos.Domain.Catalog.Enums;

namespace Ingressinhos.Application.Sales.Dtos;

public sealed class OrderItemQueryItem
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public long TicketId { get; set; }
    public string TicketName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public SeatCategory Category { get; set; }
    public long? SeatId { get; set; }
    public string? SeatCode { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
