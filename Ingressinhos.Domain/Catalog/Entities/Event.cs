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
            AddError("Evento", "Informe o nome do evento.");
        }
        else
        {
            Name = name;
        }

        if (startTime <= DateTime.Now)
        {
            AddError("Inicio", "Informe uma data de inicio valida.");
        }
        else
        {
            StartTime = startTime;
        }

        if (endTime <= startTime)
        {
            AddError("Fim", "A data de termino precisa ser posterior ao inicio.");
        }
        else
        {
            EndTime = endTime;
        }

        if (locationId <= 0)
        {
            AddError("Local", "Informe o local do evento.");
        }
        else
        {
            LocationId = locationId;
        }

        if (sellerId <= 0)
        {
            AddError("Loja", "Nao foi possivel identificar a loja responsavel pelo evento.");
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
            AddError("Evento", "Informe o nome do evento.");
            return;
        }
        
        Name = name;
    } 

    public void ChangeLocation(long locationId)
    {
        ClearErrors();

        if (locationId <= 0)
        {
            AddError("Local", "Informe o local do evento.");
            return;
        }
        
        LocationId = locationId;
    }

    public void RescheduleEvent(DateTime novaDataHoraInicio, DateTime novaDataHoraFim)
    {
        ClearErrors();

        if (novaDataHoraInicio <= DateTime.Now)
        {
            AddError("Inicio", "Informe uma data de inicio valida no futuro.");
            return;
        }

        if (novaDataHoraFim <= novaDataHoraInicio)
        {
            AddError("Fim", "A data de termino precisa ser posterior ao inicio.");
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
