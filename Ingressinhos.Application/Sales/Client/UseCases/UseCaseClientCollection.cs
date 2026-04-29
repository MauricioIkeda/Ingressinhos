using Generic.Application.Crud.UseCases;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.Interfaces;
using ClientDomain = Ingressinhos.Domain.Sales.Entities.Client;

namespace Ingressinhos.Application.Sales.UseCases;

public class UseCaseClientCollection : UseCaseCrudCollection<ClientDomain, ClientDto>, IUseCaseClientCollection
{
    public UseCaseClientCollection(IRepositorySession repositorySession, ClientUpdate update, ClientInclude include)
        : base(include, update, new UseCaseGetOdata<ClientDomain>(), new UseCaseGet<ClientDomain>(), new UseCaseDelete<ClientDomain>(), repositorySession)
    {
    }
}
