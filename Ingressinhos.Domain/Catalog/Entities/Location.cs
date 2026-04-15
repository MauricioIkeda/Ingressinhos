using Generic.Domain.Entities;

namespace Ingressinhos.Domain.Catalog.Entities;

public class Location : BaseEntity
{
    public string Name { get; private set; }
    public int TotalCapacity { get; private set; }
    public bool HasSeats { get; private set; }

    public Location(string name, int totalCapacity, bool hasSeats = true)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new Exception("Deve ser informado o nome do local");
        }

        if (totalCapacity <= 0)
        {
            throw new Exception("Deve ser informado uma capacidade valida do local");
        }
        
        Name = name;
        TotalCapacity = totalCapacity;
        HasSeats = hasSeats;
    }

    public void ChangeSeatMode(bool hasSeats)
    {
        HasSeats = hasSeats;
    }
}