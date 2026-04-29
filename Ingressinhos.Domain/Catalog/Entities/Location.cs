using Generic.Domain.Entities;

namespace Ingressinhos.Domain.Catalog.Entities;

public class Location : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public int TotalCapacity { get; private set; }
    public bool HasSeats { get; private set; }

    protected Location()
    {
    }

    public Location(string name, int totalCapacity, bool hasSeats = true)
    {
        if (string.IsNullOrEmpty(name))
        {
            AddError("Name", "Deve ser informado o nome do local");
        }
        else
        {
            Name = name;
        }

        if (totalCapacity <= 0)
        {
            AddError("TotalCapacity", "Deve ser informado uma capacidade valida do local");
        }
        else
        {
            TotalCapacity = totalCapacity;
        }
        
        HasSeats = hasSeats;
    }
    
    public void ChangeName(string name)
    {
        ClearErrors();

        if (string.IsNullOrEmpty(name))
        {
            AddError("Name", "Deve ser informado o nome do local");
            return;
        }
        
        Name = name;
    }

    public void ChangeTotalCapacity(int totalCapacity)
    {
        ClearErrors();

        if (totalCapacity <= 0)
        {
            AddError("TotalCapacity", "Deve ser informado uma capacidade valida");
            return;
        }
        
        TotalCapacity = totalCapacity;
    }

    public void ChangeSeatMode(bool hasSeats)
    {
        ClearErrors();
        HasSeats = hasSeats;
    }
}
