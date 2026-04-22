namespace Ingressinhos.Application.Catalog.Dtos;

public class EventDto
{
    public long EventId { get; set; }
    public string Name { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public long LocationId { get; set; }
    public bool HasSeats { get; set; }
}