using Generic.Application.Utils.Interface;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.UseCases;

public class SellerUpdate
{
    private readonly IRepositorySession _repositorySession;
    private readonly IRequestAuth _requestAuth;

    public SellerUpdate(IRepositorySession repositorySession, IRequestAuth requestAuth)
    {
        _repositorySession = repositorySession;
        _requestAuth = requestAuth;
    }

    public bool Execute(SellerDto seller)
    {
        if (seller is null)
        {
            throw new Exception("Deve ser informado o vendedor");
        }

        if (seller.SellerId <= 0)
        {
            throw new Exception("Deve ser informado o identificador do vendedor");
        }

        var repositoryQuery = _repositorySession.GetRepositoryQuery();
        var sellerEntity = repositoryQuery.Return<Seller>(seller.SellerId);

        if (sellerEntity is null)
        {
            throw new Exception("Vendedor nao encontrado");
        }

        if( seller.Name != sellerEntity.Name)
            sellerEntity.ChangeName(seller.Name);
        if( seller.Email != sellerEntity.Email.Endereco)
        {
            sellerEntity.ChangeEmail(seller.Email);
            if(!_requestAuth.ChangeEmail(sellerEntity.UserId, seller.Email).GetAwaiter().GetResult())
            {
                throw new Exception("Falha ao atualizar o email do usu�rio");
            }
        }
        if( seller.Cnpj != sellerEntity.Cnpj.Numero)
            sellerEntity.ChangeTradingName(seller.TradingName);

        sellerEntity.UpdatedAt = DateTime.UtcNow;

        var repository = _repositorySession.GetRepository();
        repository.Upsert(sellerEntity);
        repository.Flush().GetAwaiter().GetResult();
        return true;
    }
}