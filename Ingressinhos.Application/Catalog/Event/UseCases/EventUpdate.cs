using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using LocationDomain = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.UseCases;

public class EventUpdate
{
    private readonly IRepositorySession _repositorySession;

    public EventUpdate(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(EventDto eventDto)
    {
        if (eventDto is null)
        {
            throw new Exception("Deve ser informado o evento");
        }

        if (eventDto.EventId <= 0)
        {
            throw new Exception("Deve ser informado o identificador do evento");
        }

        var repositoryQuery = _repositorySession.GetRepositoryQuery();
        var eventEntity = repositoryQuery.Return<Event>(eventDto.EventId);

        if (eventEntity is null)
        {
            throw new Exception("Evento nao encontrado");
        }

        if (repositoryQuery.Return<LocationDomain>(eventDto.LocationId) is null)
        {
            throw new Exception("Localizacao informada nao existe");
        }

        var hasConflictingEvent = repositoryQuery.Query<Event>(
            existingEvent => existingEvent.Id != eventDto.EventId &&
                             existingEvent.LocationId == eventDto.LocationId &&
                             eventDto.StartTime < existingEvent.EndTime &&
                             eventDto.EndTime > existingEvent.StartTime)
            .Any();

        if (hasConflictingEvent)
        {
            throw new Exception("Ja existe evento para o local no intervalo informado");
        }

        if (eventDto.Name != eventEntity.Name)
        {
            eventEntity.ChangeName(eventDto.Name);
        }

        if (eventDto.LocationId != eventEntity.LocationId)
        {
            eventEntity.ChangeLocation(eventDto.LocationId);
        }

        if (eventDto.StartTime != eventEntity.StartTime || eventDto.EndTime != eventEntity.EndTime)
        {
            eventEntity.RescheduleEvent(eventDto.StartTime, eventDto.EndTime);
        }

        if (eventDto.HasSeats != eventEntity.HasSeats)
        {
            eventEntity.ChangeSeatMode(eventDto.HasSeats);
        }

        eventEntity.UpdatedAt = DateTime.UtcNow;

        var repository = _repositorySession.GetRepository();
        repository.Upsert(eventEntity);
        repository.Flush().GetAwaiter().GetResult();
        return true;
    }
}