using Generic.Domain.Entities;

namespace Ingressinhos.Domain.Catalog.Entities;

public class Event : BaseEntity
{
    public string Name { get; private set; }
    public DateTime StarTime { get; private set; }
    public Guid LocationId { get; private set; }
    public bool HasSeats { get; private set; }

    public Event(string name, DateTime startTime, Guid locationId, bool hasSeats = true)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new Exception("Deve ser informado o nome do evento");
        }

        if (startTime <= DateTime.Now)
        {
            throw new Exception("Deve ser informado uma data valida para o evento");
        }

        if (locationId == Guid.Empty)
        {
            throw  new Exception("Deve ser informado uma localidade");
        }
        
        Name = name;
        StarTime = startTime;
        LocationId = locationId;
        HasSeats = hasSeats;
    }

    public void RemarcarEvento(DateTime novaDataHora)
    {
        if (novaDataHora <= DateTime.Now)
        {
            throw new Exception("Deve ser informado uma data valida pro futuro");
        }
        
        StarTime = novaDataHora;
    }

    public void ChangeSeatMode(bool hasSeats)
    {
        HasSeats = hasSeats;
    }
}