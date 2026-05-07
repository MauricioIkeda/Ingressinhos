using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using ClientDomain = Ingressinhos.Domain.Sales.Entities.Client;

namespace Ingressinhos.Application.Sales.UseCases;

public class ClientRecover
{
    private readonly IRepositorySession _repositorySession;
    private readonly IRequestAuth _requestAuth;
    private readonly ICurrentUserContext _currentUserContext;

    public ClientRecover(IRepositorySession repositorySession, IRequestAuth requestAuth, ICurrentUserContext currentUserContext)
    {
        _repositorySession = repositorySession;
        _requestAuth = requestAuth;
        _currentUserContext = currentUserContext;
    }

    public OperationResult Execute(long clientId)
    {
        if (clientId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Id", "Deve ser informado um Id valido."));
        }

        if (!_currentUserContext.IsAuthenticated)
        {
            return OperationResult.Unauthorized(new MensagemErro("Usuario", "Usuario nao autenticado."));
        }

        if (_currentUserContext.Role != "Admin")
        {
            return OperationResult.Forbidden(new MensagemErro("Usuario", "Somente administradores podem recuperar contas de cliente."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var clientEntity = repositoryQuery.Return<ClientDomain>(clientId);

            if (clientEntity is null)
            {
                return OperationResult.NotFound(new MensagemErro("Id", "Cliente nao encontrado."));
            }

            if (clientEntity.Active)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("Client", "A conta do cliente ja esta ativa."));
            }

            var authResult = _requestAuth.ActivateUser(clientEntity.UserId).GetAwaiter().GetResult();
            if (!authResult.Success)
            {
                return authResult;
            }

            clientEntity.Activate();
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
