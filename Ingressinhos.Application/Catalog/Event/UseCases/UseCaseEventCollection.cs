using Generic.Application.Crud.UseCases;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Application.Catalog.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.UseCases;

public class UseCaseEventCollection : UseCaseCrudCollection<Event, EventDto>, IUseCaseEventCollection
{
    private readonly EventGetWithTickets _eventGetWithTickets;
    private readonly EventGetSeats _eventGetSeats;

    public UseCaseEventCollection(
        IRepositorySession repositorySession,
        IReadRepositoryQuery readRepositoryQuery,
        EventUpdate update,
        EventInclude include,
        EventGetWithTickets eventGetWithTickets,
        EventGetSeats eventGetSeats)
        : base(include, update, new UseCaseGetOdata<Event>(), new UseCaseGetId<Event>(), new UseCaseDelete<Event>(), repositorySession, readRepositoryQuery)
    {
        _eventGetWithTickets = eventGetWithTickets;
        _eventGetSeats = eventGetSeats;
    }

    public OperationResult<List<TOutput>> GetWithTicketsResult<TOutput>(Func<IQueryable<EventWithTicketsDto>, IQueryable<TOutput>> query)
    {
        return _eventGetWithTickets.Execute(query);
    }

    public OperationResult<List<EventSeatAvailabilityDto>> GetSeats(long eventId)
    {
        return _eventGetSeats.Execute(eventId);
    }
}
