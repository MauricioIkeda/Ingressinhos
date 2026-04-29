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

    public SellerUpdate(IRepositorySession repositorySession, IRequestAuth requestAuth)
    {
        _repositorySession = repositorySession;
        _requestAuth = requestAuth;
    }

    public OperationResult Execute(SellerDto seller)
    {
        if (seller is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Seller", "Deve ser informado o vendedor."));
        }

        if (seller.SellerId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Id", "Deve ser informado o identificador do vendedor."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var sellerEntity = repositoryQuery.Return<Seller>(seller.SellerId);

            if (sellerEntity is null)
            {
                return OperationResult.NotFound(new MensagemErro("Id", "Vendedor nao encontrado."));
            }

            if (seller.Name != sellerEntity.Name)
                sellerEntity.ChangeName(seller.Name);

            if (seller.Email != sellerEntity.Email.Endereco)
            {
                sellerEntity.ChangeEmail(seller.Email);
                if (!_requestAuth.ChangeEmail(sellerEntity.UserId, seller.Email).GetAwaiter().GetResult())
                {
                    return OperationResult.UnprocessableEntity(new MensagemErro("Email", "Falha ao atualizar o email do usuario."));
                }
            }

            if (seller.Cnpj != sellerEntity.Cnpj.Numero)
                sellerEntity.ChangeTradingName(seller.TradingName);

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
