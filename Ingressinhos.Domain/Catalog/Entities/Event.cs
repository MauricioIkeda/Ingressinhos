using Generic.Domain.Entities;

namespace Ingressinhos.Domain.Catalog.Entities;

public class Event : BaseEntity
{
    public long SellerId { get; private set; }
    public string Name { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public long LocationId { get; private set; }
    public bool HasSeats { get; private set; }

    protected Event()
    {
        
    }

    public Event(long sellerId, string name, DateTime startTime, DateTime endTime, long locationId, bool hasSeats = true)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new Exception("Deve ser informado o nome do evento");
        }

        if (startTime <= DateTime.Now)
        {
            throw new Exception("Deve ser informado uma data valida para o evento");
        }

        if (endTime <= startTime)
        {
            throw new Exception("O evento deve acabar depois de come�ar");
        }

        if (locationId <= 0)
        {
            throw  new Exception("Deve ser informado uma localidade");
        }

        if (sellerId <= 0)
        {
            throw  new Exception("Deve ser informado um vendedor valido");
        }
        
        SellerId = sellerId;
        Name = name;
        StartTime = startTime;
        EndTime = endTime;
        LocationId = locationId;
        HasSeats = hasSeats;
    }

    public void ChangeName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new Exception("Deve ser informado o nome do evento");
        }
        
        Name = name;
    } 

    public void ChangeLocation(long locationId)
    {
        if (locationId <= 0)
        {
            throw  new Exception("Deve ser informado uma localidade");
        }
        
        LocationId = locationId;
    }

    public void RescheduleEvent(DateTime novaDataHoraInicio, DateTime novaDataHoraFim)
    {
        if (novaDataHoraInicio <= DateTime.Now)
        {
            throw new Exception("Deve ser informado uma data valida pro futuro");
        }

        if (novaDataHoraFim <= novaDataHoraInicio)
        {
            throw new Exception("O evento deve acabar depois de come�ar");
        }
        
        StartTime = novaDataHoraInicio;
        EndTime = novaDataHoraFim;
    }

    public void ChangeSeatMode(bool hasSeats)
    {
        HasSeats = hasSeats;
    }
}