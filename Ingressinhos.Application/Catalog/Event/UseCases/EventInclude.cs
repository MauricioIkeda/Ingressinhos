using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using LocationDomain = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.UseCases;

public class EventInclude
{
    private readonly IRepositorySession _repositorySession;

    public EventInclude(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(EventDto eventDto)
    {
        if (eventDto is null)
        {
            throw new Exception("Deve ser informado o evento");
        }

        if (eventDto.EndTime <= eventDto.StartTime)
        {
            throw new Exception("A data de fim deve ser maior que a data de inicio");
        }

        IRepositoryQuery repositoryQuery = _repositorySession.GetRepositoryQuery();

        if(repositoryQuery.Return<LocationDomain>(eventDto.LocationId) is null)
        {
            throw new Exception("Localizacao informada nao existe");
        }

        var hasConflictingEvent = repositoryQuery.Query<Event>(
            existingEvent => existingEvent.LocationId == eventDto.LocationId &&
                             eventDto.StartTime < existingEvent.EndTime &&
                             eventDto.EndTime > existingEvent.StarTime)
            .Any();

        if (hasConflictingEvent)
        {
            throw new Exception("Ja existe evento para o local no intervalo informado");
        }


        var utcNow = DateTime.UtcNow;

        var eventEntity = new Event(eventDto.Name, eventDto.StartTime, eventDto.EndTime, eventDto.LocationId, eventDto.HasSeats)
        {
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };

        var repository = _repositorySession.GetRepository();
        repository.Include(eventEntity);
        repository.Flush().GetAwaiter().GetResult();
        return true;
    }
}