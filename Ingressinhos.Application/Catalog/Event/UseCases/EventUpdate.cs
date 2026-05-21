using Generic.Application.Crud.Interface;
using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Helpers;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using LocationDomain = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.UseCases;

public class EventUpdate : IUseCaseCommand<EventDto>
{
    private readonly IRepositorySession _repositorySession;
    private readonly ICurrentUserContext _currentUserContext;

    public EventUpdate(IRepositorySession repositorySession, ICurrentUserContext currentUserContext)
    {
        _repositorySession = repositorySession;
        _currentUserContext = currentUserContext;
    }

    public OperationResult Execute(EventDto eventDto)
    {
        if (eventDto is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Event", "Deve ser informado o evento."));
        }

        if (eventDto.EventId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Id", "Deve ser informado o identificador do evento."));
        }

        if (_currentUserContext.Role == "Admin" && eventDto.SellerId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("SellerId", "Deve ser informado o identificador do vendedor."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var eventEntity = repositoryQuery.Return<Event>(eventDto.EventId);

            if (eventEntity is null)
            {
                return OperationResult.NotFound(new MensagemErro("Id", "Evento nao encontrado."));
            }

            var seller = CurrentUserEntityResolver.ResolveSeller(_currentUserContext, repositoryQuery, eventDto.SellerId);
            if (seller is null)
            {
                return _currentUserContext.Role == "Admin"
                    ? OperationResult.NotFound(new MensagemErro("Id", "Vendedor nao encontrado."))
                    : OperationResult.Unauthorized(new MensagemErro("Perfil", "Nao foi possivel localizar o perfil da sua loja."));
            }

            if (!seller.Active)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("Seller", "Nao e possivel alterar evento de um vendedor desativado."));
            }

            if (eventEntity.SellerId != seller.Id)
            {
                return _currentUserContext.Role == "Admin"
                    ? OperationResult.Forbidden(new MensagemErro("Evento", "O evento informado nao pertence ao vendedor selecionado."))
                    : OperationResult.Forbidden(new MensagemErro("Evento", "Voce so pode alterar eventos da sua propria loja."));
            }

            if (repositoryQuery.Return<LocationDomain>(eventDto.LocationId) is null)
            {
                return OperationResult.NotFound(new MensagemErro("LocationId", "Localizacao informada nao existe."));
            }

            var hasConflictingEvent = repositoryQuery.Query<Event>(
                existingEvent => existingEvent.Id != eventEntity.Id &&
                                 existingEvent.LocationId == eventDto.LocationId &&
                                 eventDto.StartTime < existingEvent.EndTime &&
                                 eventDto.EndTime > existingEvent.StartTime)
                .Any();

            if (hasConflictingEvent)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("Period", "Ja existe evento para o local no intervalo informado."));
            }

            if (eventDto.Name != eventEntity.Name)
            {
                eventEntity.ChangeName(eventDto.Name);
                if (!eventEntity.IsValid)
                {
                    return eventEntity.ToUnprocessableEntityResult();
                }
            }

            if (eventDto.LocationId != eventEntity.LocationId)
            {
                eventEntity.ChangeLocation(eventDto.LocationId);
                if (!eventEntity.IsValid)
                {
                    return eventEntity.ToUnprocessableEntityResult();
                }
            }

            if (eventDto.StartTime != eventEntity.StartTime || eventDto.EndTime != eventEntity.EndTime)
            {
                eventEntity.RescheduleEvent(eventDto.StartTime, eventDto.EndTime);
                if (!eventEntity.IsValid)
                {
                    return eventEntity.ToUnprocessableEntityResult();
                }
            }

            if (eventDto.HasSeats != eventEntity.HasSeats)
            {
                eventEntity.ChangeSeatMode(eventDto.HasSeats);
                if (!eventEntity.IsValid)
                {
                    return eventEntity.ToUnprocessableEntityResult();
                }
            }

            if (eventDto.Description != eventEntity.Description)
            {
                eventEntity.ChangeDescription(eventDto.Description);
                if (!eventEntity.IsValid)
                {
                    return eventEntity.ToUnprocessableEntityResult();
                }
            }

            if (eventDto.ImageUrl != eventEntity.ImageUrl)
            {
                eventEntity.ChangeImageUrl(eventDto.ImageUrl);
                if (!eventEntity.IsValid)
                {
                    return eventEntity.ToUnprocessableEntityResult();
                }
            }

            eventEntity.UpdatedAt = DateTime.UtcNow;

            var repository = _repositorySession.GetRepository();
            repository.Upsert(eventEntity);
            repository.Flush().GetAwaiter().GetResult();
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }

}
