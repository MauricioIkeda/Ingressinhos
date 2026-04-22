using Ingressinhos.Domain.Sales.Enums;

namespace Ingressinhos.Application.Sales.Dtos;

public class OrderDto
{
    public long OrderId { get; set; }
    public long ClientId { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
}
