using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using LocationDomain = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.UseCases;

public class EventUpdate : IUseCaseCommand<EventDto>
{
    public ListMessages Messages { get; } = new();

    private readonly IRepositorySession _repositorySession;

    public EventUpdate(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(EventDto eventDto)
    {
        Messages.Clear();

        if (eventDto is null)
        {
            Messages.Add("Deve ser informado o evento", error: true);
            return false;
        }

        if (eventDto.EventId <= 0)
        {
            Messages.Add("Deve ser informado o identificador do evento", error: true);
            return false;
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var eventEntity = repositoryQuery.Return<Event>(eventDto.EventId);

            if (eventEntity is null)
            {
                Messages.Add("Evento nao encontrado", error: true);
                return false;
            }

            if (repositoryQuery.Return<LocationDomain>(eventDto.LocationId) is null)
            {
                Messages.Add("Localizacao informada nao existe", error: true);
                return false;
            }

            var hasConflictingEvent = repositoryQuery.Query<Event>(
                existingEvent => existingEvent.Id != eventEntity.Id &&
                                 existingEvent.LocationId == eventDto.LocationId &&
                                 eventDto.StartTime < existingEvent.EndTime &&
                                 eventDto.EndTime > existingEvent.StartTime)
                .Any();

            if (hasConflictingEvent)
            {
                Messages.Add("Ja existe evento para o local no intervalo informado", error: true);
                return false;
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
        catch (Exception ex)
        {
            Messages.Add(ex);
            return false;
        }
    }
}
