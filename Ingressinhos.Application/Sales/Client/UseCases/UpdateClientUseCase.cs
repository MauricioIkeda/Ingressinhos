using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using ClientDomain = Ingressinhos.Domain.Sales.Entities.Client;

namespace Ingressinhos.Application.Sales.UseCases;

public class ClientUpdate
{
    private readonly IRepositorySession _repositorySession;

    public ClientUpdate(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(ClientDto clientDto)
    {
        if (clientDto is null)
        {
            throw new Exception("Deve ser informado o cliente");
        }

        if (clientDto.ClientId <= 0)
        {
            throw new Exception("Deve ser informado um Id valido");
        }
        
        var repositoryQuery = _repositorySession.GetRepositoryQuery();
        var clientEntity = repositoryQuery.Return<ClientDomain>(clientDto.ClientId);

        if (clientEntity is null)
        {
            throw new Exception("Cliente nao encontrado");
        }

        if (clientDto.Name != clientEntity.Name)
        {
            clientEntity.ChangeName(clientDto.Name);
        }

        if (clientDto.Email != clientEntity.Email.Endereco)
        {
            clientEntity.ChangeEmail(clientDto.Email);
        }

        clientEntity.UpdatedAt = DateTime.UtcNow;

        var repository = _repositorySession.GetRepository();
        repository.Upsert(clientEntity);
        repository.Flush().GetAwaiter().GetResult();
        return true;
    }
}