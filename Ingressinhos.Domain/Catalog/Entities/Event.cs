using Generic.Domain.Entities;

namespace Ingressinhos.Domain.Catalog.Entities;

public class Event : BaseEntity
{
    public long SellerId { get; private set; }
    public string Name { get; private set; } = string.Empty;
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
            AddError("Name", "Deve ser informado o nome do evento");
        }
        else
        {
            Name = name;
        }

        if (startTime <= DateTime.Now)
        {
            AddError("StartTime", "Deve ser informado uma data valida para o evento");
        }
        else
        {
            StartTime = startTime;
        }

        if (endTime <= startTime)
        {
            AddError("EndTime", "O evento deve acabar depois de comecar");
        }
        else
        {
            EndTime = endTime;
        }

        if (locationId <= 0)
        {
            AddError("LocationId", "Deve ser informado uma localidade");
        }
        else
        {
            LocationId = locationId;
        }

        if (sellerId <= 0)
        {
            AddError("SellerId", "Deve ser informado um vendedor valido");
        }
        else
        {
            SellerId = sellerId;
        }
        
        HasSeats = hasSeats;
    }

    public void ChangeName(string name)
    {
        ClearErrors();

        if (string.IsNullOrEmpty(name))
        {
            AddError("Name", "Deve ser informado o nome do evento");
            return;
        }
        
        Name = name;
    } 

    public void ChangeLocation(long locationId)
    {
        ClearErrors();

        if (locationId <= 0)
        {
            AddError("LocationId", "Deve ser informado uma localidade");
            return;
        }
        
        LocationId = locationId;
    }

    public void RescheduleEvent(DateTime novaDataHoraInicio, DateTime novaDataHoraFim)
    {
        ClearErrors();

        if (novaDataHoraInicio <= DateTime.Now)
        {
            AddError("StartTime", "Deve ser informado uma data valida pro futuro");
            return;
        }

        if (novaDataHoraFim <= novaDataHoraInicio)
        {
            AddError("EndTime", "O evento deve acabar depois de comecar");
            return;
        }
        
        StartTime = novaDataHoraInicio;
        EndTime = novaDataHoraFim;
    }

    public void ChangeSeatMode(bool hasSeats)
    {
        ClearErrors();
        HasSeats = hasSeats;
    }
}
