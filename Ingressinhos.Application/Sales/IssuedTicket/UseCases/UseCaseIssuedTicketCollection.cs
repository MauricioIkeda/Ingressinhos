using Generic.Application.UseCases;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.Interfaces;
using IssuedTicketDomain = Ingressinhos.Domain.Sales.Entities.IssuedTicket;

namespace Ingressinhos.Application.Sales.UseCases;

public class UseCaseIssuedTicketCollection : UseCaseCrudCollection<IssuedTicketDomain, IssuedTicketDto>, IUseCaseIssuedTicketCollection
{
    public UseCaseIssuedTicketCollection(IRepositorySession repositorySession, IssuedTicketUpdate update, IssuedTicketInclude include)
        : base(include.Execute, update.Execute, new UseCaseGetOdata<IssuedTicketDomain>(), new UseCaseGet<IssuedTicketDomain>(), new UseCaseDelete<IssuedTicketDomain>(), repositorySession)
    {
    }
}
