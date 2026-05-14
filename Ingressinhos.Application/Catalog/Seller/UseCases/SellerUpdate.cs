using Generic.Application.Crud.Interface;
using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.UseCases;

public class SellerUpdate : IUseCaseCommand<SellerDto>
{
    private readonly IRepositorySession _repositorySession;
    private readonly IRequestAuth _requestAuth;
    private readonly ICurrentUserContext _currentUserContext;

    public SellerUpdate(IRepositorySession repositorySession, IRequestAuth requestAuth, ICurrentUserContext currentUserContext)
    {
        _repositorySession = repositorySession;
        _requestAuth = requestAuth;
        _currentUserContext = currentUserContext;
    }

    public OperationResult Execute(SellerDto seller)
    {
        if (seller is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Seller", "Deve ser informado o vendedor."));
        }

        if (_currentUserContext.Role == "Admin" && seller.SellerId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Id", "Deve ser informado o identificador do vendedor."));
        }

        string? userId = null;
        string? previousEmail = null;
        var authEmailChanged = false;

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var sellerEntity = ResolveTargetSeller(seller.SellerId, repositoryQuery);

            if (sellerEntity is null)
            {
                return _currentUserContext.Role == "Admin"
                    ? OperationResult.NotFound(new MensagemErro("Id", "Vendedor nao encontrado."))
                    : OperationResult.Unauthorized(new MensagemErro("Perfil", "Nao foi possivel localizar o perfil da sua conta."));
            }

            if (!sellerEntity.Active)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("Seller", "Nao e possivel editar um vendedor desativado."));
            }

            userId = sellerEntity.UserId;

            if (seller.Name != sellerEntity.Name)
            {
                sellerEntity.ChangeName(seller.Name);
                if (!sellerEntity.IsValid)
                {
                    return sellerEntity.ToUnprocessableEntityResult();
                }
            }

            if (seller.Email != sellerEntity.Email.Endereco)
            {
                previousEmail = sellerEntity.Email.Endereco;

                sellerEntity.ChangeEmail(seller.Email);
                if (!sellerEntity.IsValid)
                {
                    return sellerEntity.ToUnprocessableEntityResult();
                }

                var authResult = _requestAuth.ChangeEmail(sellerEntity.UserId, seller.Email).GetAwaiter().GetResult();
                if (!authResult.Success)
                {
                    return authResult;
                }

                authEmailChanged = true;
            }

            if (seller.Cnpj != sellerEntity.Cnpj.Numero)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("Cnpj", "Nao e permitido alterar o CNPJ do vendedor."));
            }

            if (seller.TradingName != sellerEntity.TradingName)
            {
                sellerEntity.ChangeTradingName(seller.TradingName);
                if (!sellerEntity.IsValid)
                {
                    return sellerEntity.ToUnprocessableEntityResult();
                }
            }

            sellerEntity.UpdatedAt = DateTime.UtcNow;

            var repository = _repositorySession.GetRepository();
            repository.Upsert(sellerEntity);
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

    private Seller? ResolveTargetSeller(long sellerId, IRepositoryQuery repositoryQuery)
    {
        if (_currentUserContext.Role == "Admin")
        {
            return repositoryQuery.Return<Seller>(sellerId);
        }

        return repositoryQuery.Query<Seller>(s => s.UserId == _currentUserContext.UserId).FirstOrDefault();
    }
}
