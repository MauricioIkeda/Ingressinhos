using Generic.Application.Crud.UseCases;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Application.Catalog.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.UseCases;

public class UseCaseTicketCollection : UseCaseCrudCollection<Ticket, TicketDto>, IUseCaseTicketCollection
{
    private readonly TicketGetOdata _ticketGetOdata;

    public UseCaseTicketCollection(
        IRepositorySession repositorySession,
        TicketUpdate update,
        TicketInclude include,
        TicketGetOdata ticketGetOdata)
        : base(include, update, new UseCaseGetOdata<Ticket>(), new UseCaseGetId<Ticket>(), new UseCaseDelete<Ticket>(), repositorySession)
    {
        _ticketGetOdata = ticketGetOdata;
    }

    public OperationResult<List<TOutput>> GetQueryItems<TOutput>(Func<IQueryable<TicketQueryItem>, IQueryable<TOutput>> transaction)
    {
        return _ticketGetOdata.Execute(transaction);
    }
}
