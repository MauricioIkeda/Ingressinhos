using Generic.Application.Crud.Interface;
using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Helpers;
using Ingressinhos.Application.Sales.Dtos;
using ClientDomain = Ingressinhos.Domain.Sales.Entities.Client;

namespace Ingressinhos.Application.Sales.UseCases;

public class ClientUpdate : IUseCaseCommand<ClientDto>
{
    private readonly IRepositorySession _repositorySession;
    private readonly IRequestAuth _requestAuth;
    private readonly ICurrentUserContext _currentUserContext;

    public ClientUpdate(IRepositorySession repositorySession, IRequestAuth requestAuth, ICurrentUserContext currentUserContext)
    {
        _repositorySession = repositorySession;
        _requestAuth = requestAuth;
        _currentUserContext = currentUserContext;
    }

    public OperationResult Execute(ClientDto clientDto)
    {
        if (clientDto is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Client", "Deve ser informado o cliente."));
        }

        if (_currentUserContext.Role == "Admin" && clientDto.ClientId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Id", "Deve ser informado um Id valido."));
        }
        
        string? userId = null;
        string? previousEmail = null;
        var authEmailChanged = false;

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var clientEntity = CurrentUserEntityResolver.ResolveClient(_currentUserContext, repositoryQuery, clientDto.ClientId, onlyActive: false);

            if (clientEntity is null)
            {
                return _currentUserContext.Role == "Admin"
                    ? OperationResult.NotFound(new MensagemErro("Id", "Cliente nao encontrado."))
                    : OperationResult.Unauthorized(new MensagemErro("Perfil", "Nao foi possivel localizar o perfil da sua conta."));
            }

            if (!clientEntity.Active)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("Client", "Nao e possivel editar um cliente desativado."));
            }

            userId = clientEntity.UserId;

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
                previousEmail = clientEntity.Email.Endereco;

                clientEntity.ChangeEmail(clientDto.Email);
                if (!clientEntity.IsValid)
                {
                    return clientEntity.ToUnprocessableEntityResult();
                }

                var authResult = _requestAuth.ChangeEmail(clientEntity.UserId, clientDto.Email).GetAwaiter().GetResult();
                if (!authResult.Success)
                {
                    return authResult;
                }

                authEmailChanged = true;
            }

            clientEntity.UpdatedAt = DateTime.UtcNow;

            var repository = _repositorySession.GetRepository();
            repository.Upsert(clientEntity);
            repository.Flush().GetAwaiter().GetResult();
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            if (authEmailChanged && !string.IsNullOrWhiteSpace(previousEmail) && !string.IsNullOrWhiteSpace(userId))
            {
                _requestAuth.ChangeEmail(userId, previousEmail).GetAwaiter().GetResult();
            }

            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
