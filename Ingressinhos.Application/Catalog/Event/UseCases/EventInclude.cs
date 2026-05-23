using Generic.Application.Crud.Interface;
using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Helpers;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using LocationDomain = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.UseCases;

public class EventInclude : IUseCaseCommand<EventDto>, IUseCaseCommand<EventDto, EventDto>
{
    private readonly IRepositorySession _repositorySession;
    private readonly ICurrentUserContext _currentUserContext;

    public EventInclude(IRepositorySession repositorySession, ICurrentUserContext currentUserContext)
    {
        _repositorySession = repositorySession;
        _currentUserContext = currentUserContext;
    }

    OperationResult IUseCaseCommand<EventDto>.Execute(EventDto eventDto)
    {
        return Execute(eventDto);
    }

    public OperationResult<EventDto> Execute(EventDto eventDto)
    {
        if (eventDto is null)
        {
            return OperationResult<EventDto>.UnprocessableEntity(new MensagemErro("Evento", "Envie os dados do evento."));
        }

        if (_currentUserContext.Role == "Admin" && eventDto.SellerId <= 0)
        {
            return OperationResult<EventDto>.UnprocessableEntity(new MensagemErro("SellerId", "Deve ser informado o identificador do vendedor."));
        }

        try
        {
            IRepositoryQuery repositoryQuery = _repositorySession.GetRepositoryQuery();
            var seller = CurrentUserEntityResolver.ResolveSeller(_currentUserContext, repositoryQuery, eventDto.SellerId);
            if (seller is null)
            {
                return _currentUserContext.Role == "Admin"
                    ? OperationResult<EventDto>.NotFound(new MensagemErro("Id", "Vendedor nao encontrado."))
                    : OperationResult<EventDto>.Unauthorized(new MensagemErro("Perfil", "Nao foi possivel localizar o perfil da sua loja."));
            }

            if (!seller.Active)
            {
                return OperationResult<EventDto>.UnprocessableEntity(new MensagemErro("Seller", "Nao e possivel cadastrar evento para um vendedor desativado."));
            }

            if (repositoryQuery.Return<LocationDomain>(eventDto.LocationId) is null)
            {
                return OperationResult<EventDto>.NotFound(new MensagemErro("Local", "Nao encontramos o local informado."));
            }

            var hasConflictingEvent = repositoryQuery.Query<Event>(
                existingEvent => existingEvent.LocationId == eventDto.LocationId &&
                                 eventDto.StartTime < existingEvent.EndTime &&
                                 eventDto.EndTime > existingEvent.StartTime).Any();

            if (hasConflictingEvent)
            {
                return OperationResult<EventDto>.UnprocessableEntity(new MensagemErro("Agenda", "Ja existe um evento para esse local no periodo informado."));
            }

            var utcNow = DateTime.UtcNow;

            var eventEntity = new Event(seller.Id, eventDto.Name, eventDto.StartTime, eventDto.EndTime, eventDto.Description, eventDto.ImageUrl, eventDto.LocationId, eventDto.HasSeats)
            {
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };
            if (!eventEntity.IsValid)
            {
                return eventEntity.ToUnprocessableEntityResult<EventDto>();
            }

            var repository = _repositorySession.GetRepository();
            repository.Include(eventEntity);
            repository.Flush().GetAwaiter().GetResult();
            eventDto.EventId = eventEntity.Id;
            return OperationResult<EventDto>.Created(eventDto);
        }
        catch (Exception ex)
        {
            return OperationResult<EventDto>.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }

}
