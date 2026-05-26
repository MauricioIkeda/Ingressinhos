namespace Ingressinhos.Application.Sales.TicketReadModel.Dtos;

public class ClientTicketViewDto
{
    public long IssuedTicketId { get; set; }
    public string AccessCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime IssuedAtUtc { get; set; }
    public DateTime? CheckedInAtUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }
    public DateTime? PaidAtUtc { get; set; }
    public long ClientId { get; set; }
    public string ClientUserId { get; set; } = string.Empty;
    public long OrderId { get; set; }
    public long OrderItemId { get; set; }
    public string TicketName { get; set; } = string.Empty;
    public string SeatCode { get; set; }
    public string Category { get; set; } = string.Empty;
    public long EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public DateTime EventStartTimeUtc { get; set; }
    public DateTime EventEndTimeUtc { get; set; }
    public string EventImageUrl { get; set; } = string.Empty;
    public long LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public DateTime ProjectedAtUtc { get; set; }
}
