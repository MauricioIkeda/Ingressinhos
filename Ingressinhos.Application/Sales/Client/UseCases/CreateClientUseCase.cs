using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using ClientDomain = Ingressinhos.Domain.Sales.Entities.Client;

namespace Ingressinhos.Application.Sales.UseCases;

public class ClientInclude
{
    private readonly IRepositorySession _repositorySession;

    public ClientInclude(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(ClientDto clientDto)
    {
        if (clientDto is null)
        {
            throw new Exception("Deve informar o cliente");
        }
        
        var utcNow = DateTime.UtcNow;

        var clientEntity = new ClientDomain(clientDto.Name, clientDto.Email, clientDto.Cpf)
        {
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };
        
        var repository = _repositorySession.GetRepository();
        repository.Include(clientEntity);
        repository.Flush().GetAwaiter().GetResult();
        return true;
    }
}