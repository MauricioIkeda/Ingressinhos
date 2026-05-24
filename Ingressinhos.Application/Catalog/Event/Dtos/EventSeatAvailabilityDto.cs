using Ingressinhos.Domain.Catalog.Enums;

namespace Ingressinhos.Application.Catalog.Dtos;

public sealed class EventSeatAvailabilityDto
{
    public long SeatId { get; set; }
    public string Code { get; set; } = string.Empty;
    public SeatCategory Category { get; set; }
    public SeatStatus Status { get; set; }
}
