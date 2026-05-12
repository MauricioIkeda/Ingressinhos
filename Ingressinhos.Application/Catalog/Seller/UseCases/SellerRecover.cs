using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.UseCases;

public class SellerRecover
{
    private readonly IRepositorySession _repositorySession;
    private readonly IRequestAuth _requestAuth;
    private readonly ICurrentUserContext _currentUserContext;

    public SellerRecover(IRepositorySession repositorySession, IRequestAuth requestAuth, ICurrentUserContext currentUserContext)
    {
        _repositorySession = repositorySession;
        _requestAuth = requestAuth;
        _currentUserContext = currentUserContext;
    }

    public OperationResult Execute(long sellerId)
    {
        if (sellerId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Id", "Deve ser informado o identificador do vendedor."));
        }

        if (!_currentUserContext.IsAuthenticated)
        {
            return OperationResult.Unauthorized(new MensagemErro("Usuario", "Usuario nao autenticado."));
        }

        if (_currentUserContext.Role != "Admin")
        {
            return OperationResult.Forbidden(new MensagemErro("Usuario", "Somente administradores podem recuperar contas de vendedor."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var sellerEntity = repositoryQuery.Return<Seller>(sellerId);

            if (sellerEntity is null)
            {
                return OperationResult.NotFound(new MensagemErro("Id", "Vendedor nao encontrado."));
            }

            if (sellerEntity.Active)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("Seller", "A conta do vendedor ja esta ativa."));
            }

            var authResult = _requestAuth.ActivateUser(sellerEntity.UserId).GetAwaiter().GetResult();
            if (!authResult.Success)
            {
                return authResult;
            }

            sellerEntity.Activate();
            if (!sellerEntity.IsValid)
            {
                return sellerEntity.ToUnprocessableEntityResult();
            }

            sellerEntity.UpdatedAt = DateTime.UtcNow;

            var repository = _repositorySession.GetRepository();
            repository.Upsert(sellerEntity);
            repository.Flush().GetAwaiter().GetResult();
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
