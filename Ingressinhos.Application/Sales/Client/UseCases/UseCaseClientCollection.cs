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
    private readonly ClientGetByToken _clientGetByToken;
    private readonly ClientGetOdata _clientGetOdata;

    public UseCaseClientCollection(
        IRepositorySession repositorySession,
        IReadRepositoryQuery readRepositoryQuery,
        ClientUpdate update,
        ClientInclude include,
        ClientDeactivate clientDeactivate,
        ClientRecover clientRecover,
        ClientGetByToken clientGetByToken,
        ClientGetOdata clientGetOdata)
        : base(include, update, new UseCaseGetOdata<ClientDomain>(), new UseCaseGetId<ClientDomain>(), new UseCaseDelete<ClientDomain>(), repositorySession, readRepositoryQuery)
    {
        _clientDeactivate = clientDeactivate;
        _clientRecover = clientRecover;
        _clientGetByToken = clientGetByToken;
        _clientGetOdata = clientGetOdata;
    }

    public OperationResult Deactivate(long id)
    {
        return _clientDeactivate.Execute(id);
    }

    public OperationResult Recover(long id)
    {
        return _clientRecover.Execute(id);
    }

    public OperationResult<ClientGet> GetByToken()
    {
        return _clientGetByToken.Execute();
    }

    public OperationResult<List<TOutput>> GetQueryItems<TOutput>(Func<IQueryable<ClientQueryItem>, IQueryable<TOutput>> transaction)
    {
        return _clientGetOdata.Execute(transaction);
    }
}
