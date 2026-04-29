using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using LocationDomain = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.UseCases;

public class EventInclude : IUseCaseCommand<EventDto>
{
    private readonly IRepositorySession _repositorySession;

    public EventInclude(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult Execute(EventDto eventDto)
    {
        if (eventDto is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Event", "Deve ser informado o evento."));
        }

        try
        {
            IRepositoryQuery repositoryQuery = _repositorySession.GetRepositoryQuery();

            if (repositoryQuery.Return<LocationDomain>(eventDto.LocationId) is null)
            {
                return OperationResult.NotFound(new MensagemErro("LocationId", "Localizacao informada nao existe."));
            }

            var hasConflictingEvent = repositoryQuery.Query<Event>(
                existingEvent => existingEvent.LocationId == eventDto.LocationId &&
                                 eventDto.StartTime < existingEvent.EndTime &&
                                 eventDto.EndTime > existingEvent.StartTime)
                .Any();

            if (hasConflictingEvent)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("Period", "Ja existe evento para o local no intervalo informado."));
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
            return OperationResult.Created();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
