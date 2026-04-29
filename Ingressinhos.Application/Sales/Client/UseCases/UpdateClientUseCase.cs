using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using ClientDomain = Ingressinhos.Domain.Sales.Entities.Client;

namespace Ingressinhos.Application.Sales.UseCases;

public class ClientUpdate : IUseCaseCommand<ClientDto>
{
    public ListMessages Messages { get; } = new();

    private readonly IRepositorySession _repositorySession;

    public ClientUpdate(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(ClientDto clientDto)
    {
        Messages.Clear();

        if (clientDto is null)
        {
            Messages.Add("Deve ser informado o cliente", error: true);
            return false;
        }

        if (clientDto.ClientId <= 0)
        {
            Messages.Add("Deve ser informado um Id valido", error: true);
            return false;
        }
        
        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var clientEntity = repositoryQuery.Return<ClientDomain>(clientDto.ClientId);

            if (clientEntity is null)
            {
                Messages.Add("Cliente nao encontrado", error: true);
                return false;
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
        catch (Exception ex)
        {
            Messages.Add(ex);
            return false;
        }
    }
}
