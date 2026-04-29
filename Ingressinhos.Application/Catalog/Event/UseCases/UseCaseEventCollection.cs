using Generic.Application.Crud.UseCases;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Application.Catalog.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.UseCases;

public class UseCaseEventCollection : UseCaseCrudCollection<Event, EventDto>, IUseCaseEventCollection
{
    public UseCaseEventCollection(IRepositorySession repositorySession, EventUpdate update, EventInclude include)
        : base(include, update, new UseCaseGetOdata<Event>(), new UseCaseGet<Event>(), new UseCaseDelete<Event>(), repositorySession)
    {
    }
}
