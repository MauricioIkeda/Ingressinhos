using Generic.Domain.Entities;

namespace Ingressinhos.Domain.Catalog.Entities;

public class Location : BaseEntity
{
    public string Name { get; private set; }
    public int TotalCapacity { get; private set; }
    public bool HasSeats { get; private set; }

    protected Location()
    {
    }

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
    
    public void ChangeName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new Exception("Deve ser informado o nome do local");
        }
        
        Name = name;
    }

    public void ChangeTotalCapacity(int totalCapacity)
    {
        if (totalCapacity <= 0)
        {
            throw new Exception("Deve ser informado uma capacidade valida");
        }
        
        TotalCapacity = totalCapacity;
    }

    public void ChangeSeatMode(bool hasSeats)
    {
        HasSeats = hasSeats;
    }
}