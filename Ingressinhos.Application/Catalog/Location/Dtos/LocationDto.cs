namespace Ingressinhos.Application.Catalog.Location.Dtos;

public class LocationDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public int TotalCapacity { get; set; }
    public bool HasSeats { get; set; }
}