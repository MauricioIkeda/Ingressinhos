using Generic.Application.Crud.Interface;
using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using ClientDomain = Ingressinhos.Domain.Sales.Entities.Client;

namespace Ingressinhos.Application.Sales.UseCases;

public class ClientInclude : IUseCaseCommand<ClientDto>
{
    private readonly IRepositorySession _repositorySession;
    private readonly IRequestAuth _requestAuth;

    public ClientInclude(IRepositorySession repositorySession, IRequestAuth requestAuth)
    {
        _repositorySession = repositorySession;
        _requestAuth = requestAuth;
    }

    public OperationResult Execute(ClientDto clientDto)
    {
        if (clientDto is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Client", "Deve informar o cliente."));
        }
        
        try
        {
            var utcNow = DateTime.UtcNow;

            string userId = _requestAuth.CreateUser(clientDto.Name, clientDto.Email, clientDto.Password, 2)
                .GetAwaiter().GetResult();

            var clientEntity = new ClientDomain(clientDto.Name, clientDto.Email, clientDto.Cpf, userId)
            {
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };
            if (!clientEntity.IsValid)
            {
                return clientEntity.ToUnprocessableEntityResult();
            }
            
            var repository = _repositorySession.GetRepository();
            repository.Include(clientEntity);
            repository.Flush().GetAwaiter().GetResult();
            return OperationResult.Created();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
