using Ingressinhos.Domain.Sales.Enums;

namespace Ingressinhos.Application.Sales.Dtos;

public class IssuedTicketDto
{
    public long IssuedTicketId { get; set; }
    public long OrderItemId { get; set; }
    public long ClientId { get; set; }
    public long EventId { get; set; }
    public string AccessCode { get; set; }
    public IssuedTicketStatus Status { get; set; }
}
