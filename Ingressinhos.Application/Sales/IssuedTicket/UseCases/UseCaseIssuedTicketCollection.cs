using Generic.Application.Crud.UseCases;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.Dtos;
using Ingressinhos.Application.Sales.TicketReadModel.Interfaces;
using IssuedTicketDomain = Ingressinhos.Domain.Sales.Entities.IssuedTicket;

namespace Ingressinhos.Application.Sales.UseCases;

public class UseCaseIssuedTicketCollection : UseCaseQueryCollection<IssuedTicketDomain>, IUseCaseIssuedTicketCollection
{
    private readonly IUseCaseGetMyClientTickets _getMyClientTickets;

    public UseCaseIssuedTicketCollection(
        IRepositorySession repositorySession,
        IReadRepositoryQuery readRepositoryQuery,
        IUseCaseGetMyClientTickets getMyClientTickets)
        : base(new UseCaseGetOdata<IssuedTicketDomain>(), new UseCaseGetId<IssuedTicketDomain>(), repositorySession, readRepositoryQuery)
    {
        _getMyClientTickets = getMyClientTickets;
    }

    public OperationResult<List<TOutput>> GetMyTickets<TOutput>(Func<IQueryable<ClientTicketViewDto>, IQueryable<TOutput>> query)
    {
        return _getMyClientTickets.Execute(query);
    }
}
