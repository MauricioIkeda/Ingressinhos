using Generic.Application.Crud.Interface;
using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using ClientDomain = Ingressinhos.Domain.Sales.Entities.Client;

namespace Ingressinhos.Application.Sales.UseCases;

public class ClientInclude : IUseCaseCommand<ClientDto>
{
    public ListMessages Messages { get; } = new();

    private readonly IRepositorySession _repositorySession;
    private readonly IRequestAuth _requestAuth;

    public ClientInclude(IRepositorySession repositorySession, IRequestAuth requestAuth)
    {
        _repositorySession = repositorySession;
        _requestAuth = requestAuth;
    }

    public bool Execute(ClientDto clientDto)
    {
        Messages.Clear();

        if (clientDto is null)
        {
            Messages.Add("Deve informar o cliente", error: true);
            return false;
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
            
            var repository = _repositorySession.GetRepository();
            repository.Include(clientEntity);
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
