using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using LocationDomain = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.UseCases;

public class EventInclude : IUseCaseCommand<EventDto>
{
    public ListMessages Messages { get; } = new();

    private readonly IRepositorySession _repositorySession;

    public EventInclude(IRepositorySession repositorySession)
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

        try
        {
            IRepositoryQuery repositoryQuery = _repositorySession.GetRepositoryQuery();

            if (repositoryQuery.Return<LocationDomain>(eventDto.LocationId) is null)
            {
                Messages.Add("Localizacao informada nao existe", error: true);
                return false;
            }

            var hasConflictingEvent = repositoryQuery.Query<Event>(
                existingEvent => existingEvent.LocationId == eventDto.LocationId &&
                                 eventDto.StartTime < existingEvent.EndTime &&
                                 eventDto.EndTime > existingEvent.StartTime)
                .Any();

            if (hasConflictingEvent)
            {
                Messages.Add("Ja existe evento para o local no intervalo informado", error: true);
                return false;
            }

            var utcNow = DateTime.UtcNow;

            var eventEntity = new Event(eventDto.SellerId, eventDto.Name, eventDto.StartTime, eventDto.EndTime, eventDto.LocationId, eventDto.HasSeats)
            {
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };

            var repository = _repositorySession.GetRepository();
            repository.Include(eventEntity);
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
