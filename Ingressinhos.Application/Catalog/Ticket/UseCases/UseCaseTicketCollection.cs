using Generic.Application.UseCases;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Application.Catalog.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.UseCases;

public class UseCaseTicketCollection : UseCaseCrudCollection<Ticket, TicketDto>, IUseCaseTicketCollection
{
    public UseCaseTicketCollection(IRepositorySession repositorySession, TicketUpdate update, TicketInclude include)
        : base(include.Execute, update.Execute, new UseCaseGetOdata<Ticket>(), new UseCaseGet<Ticket>(), new UseCaseDelete<Ticket>(), repositorySession)
    {
    }
}