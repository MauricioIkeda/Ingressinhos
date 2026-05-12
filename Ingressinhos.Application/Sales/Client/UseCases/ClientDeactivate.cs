using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using ClientDomain = Ingressinhos.Domain.Sales.Entities.Client;

namespace Ingressinhos.Application.Sales.UseCases;

public class ClientDeactivate
{
    private readonly IRepositorySession _repositorySession;
    private readonly IRequestAuth _requestAuth;
    private readonly ICurrentUserContext _currentUserContext;

    public ClientDeactivate(IRepositorySession repositorySession, IRequestAuth requestAuth, ICurrentUserContext currentUserContext)
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

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var clientEntity = repositoryQuery.Return<ClientDomain>(clientId);

            if (clientEntity is null)
            {
                return OperationResult.NotFound(new MensagemErro("Id", "Cliente nao encontrado."));
            }

            if (_currentUserContext.Role != "Admin" && _currentUserContext.UserId != clientEntity.UserId)
            {
                return OperationResult.Forbidden(new MensagemErro("Usuario", "Voce nao pode desativar a conta de outro cliente."));
            }

            if (!clientEntity.Active)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("Client", "A conta do cliente ja esta desativada."));
            }

            var authResult = _requestAuth.DeactivateUser(clientEntity.UserId).GetAwaiter().GetResult();
            if (!authResult.Success)
            {
                return authResult;
            }

            clientEntity.Deactivate();
            if (!clientEntity.IsValid)
            {
                return clientEntity.ToUnprocessableEntityResult();
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
