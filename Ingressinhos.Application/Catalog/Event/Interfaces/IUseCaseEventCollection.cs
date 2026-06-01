using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.Interfaces;

public interface IUseCaseEventCollection : IUseCaseCrudCollection<Event, EventDto>
{
    OperationResult<List<TOutput>> GetWithTicketsResult<TOutput>(Func<IQueryable<EventWithTicketsDto>, IQueryable<TOutput>> query);
    OperationResult<List<EventSeatAvailabilityDto>> GetSeats(long eventId);
    OperationResult DeleteEvent(long eventId);
}
