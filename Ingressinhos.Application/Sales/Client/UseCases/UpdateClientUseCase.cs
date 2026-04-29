using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using ClientDomain = Ingressinhos.Domain.Sales.Entities.Client;

namespace Ingressinhos.Application.Sales.UseCases;

public class ClientUpdate : IUseCaseCommand<ClientDto>
{
    private readonly IRepositorySession _repositorySession;

    public ClientUpdate(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult Execute(ClientDto clientDto)
    {
        if (clientDto is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Client", "Deve ser informado o cliente."));
        }

        if (clientDto.ClientId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Id", "Deve ser informado um Id valido."));
        }
        
        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var clientEntity = repositoryQuery.Return<ClientDomain>(clientDto.ClientId);

            if (clientEntity is null)
            {
                return OperationResult.NotFound(new MensagemErro("Id", "Cliente nao encontrado."));
            }

            if (clientDto.Name != clientEntity.Name)
            {
                clientEntity.ChangeName(clientDto.Name);
                if (!clientEntity.IsValid)
                {
                    return clientEntity.ToUnprocessableEntityResult();
                }
            }

            if (clientDto.Email != clientEntity.Email.Endereco)
            {
                clientEntity.ChangeEmail(clientDto.Email);
                if (!clientEntity.IsValid)
                {
                    return clientEntity.ToUnprocessableEntityResult();
                }
            }

            clientEntity.UpdatedAt = DateTime.UtcNow;

            var repository = _repositorySession.GetRepository();
            repository.Upsert(clientEntity);
            repository.Flush().GetAwaiter().GetResult();
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
