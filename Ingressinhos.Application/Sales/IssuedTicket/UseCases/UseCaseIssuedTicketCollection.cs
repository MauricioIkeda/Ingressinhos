using Generic.Application.Crud.UseCases;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.Interfaces;
using IssuedTicketDomain = Ingressinhos.Domain.Sales.Entities.IssuedTicket;

namespace Ingressinhos.Application.Sales.UseCases;

public class UseCaseIssuedTicketCollection : UseCaseCrudCollection<IssuedTicketDomain, IssuedTicketDto>, IUseCaseIssuedTicketCollection
{
    public UseCaseIssuedTicketCollection(IRepositorySession repositorySession, IReadRepositoryQuery readRepositoryQuery, IssuedTicketUpdate update, IssuedTicketInclude include)
        : base(include, update, new UseCaseGetOdata<IssuedTicketDomain>(), new UseCaseGetId<IssuedTicketDomain>(), new UseCaseDelete<IssuedTicketDomain>(), repositorySession, readRepositoryQuery)
    {
    }
}
