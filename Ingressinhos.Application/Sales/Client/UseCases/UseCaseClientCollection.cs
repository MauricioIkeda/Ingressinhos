using Generic.Application.Crud.UseCases;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.Interfaces;
using ClientDomain = Ingressinhos.Domain.Sales.Entities.Client;

namespace Ingressinhos.Application.Sales.UseCases;

public class UseCaseClientCollection : UseCaseCrudCollection<ClientDomain, ClientDto>, IUseCaseClientCollection
{
    private readonly ClientDeactivate _clientDeactivate;
    private readonly ClientRecover _clientRecover;

    public UseCaseClientCollection(
        IRepositorySession repositorySession,
        ClientUpdate update,
        ClientInclude include,
        ClientDeactivate clientDeactivate,
        ClientRecover clientRecover)
        : base(include, update, new UseCaseGetOdata<ClientDomain>(), new UseCaseGet<ClientDomain>(), new UseCaseDelete<ClientDomain>(), repositorySession)
    {
        _clientDeactivate = clientDeactivate;
        _clientRecover = clientRecover;
    }

    public OperationResult Deactivate(long id)
    {
        return _clientDeactivate.Execute(id);
    }

    public OperationResult Recover(long id)
    {
        return _clientRecover.Execute(id);
    }
}
