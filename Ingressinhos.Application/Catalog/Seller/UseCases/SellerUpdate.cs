using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.UseCases;

public class SellerUpdate
{
    private readonly IRepositorySession _repositorySession;

    public SellerUpdate(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
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
            sellerEntity.ChangeEmail(seller.Email);
        if( seller.Cnpj != sellerEntity.Cnpj.Numero)
            sellerEntity.ChangeTradingName(seller.TradingName);

        sellerEntity.UpdatedAt = DateTime.UtcNow;

        var repository = _repositorySession.GetRepository();
        repository.Upsert(sellerEntity);
        repository.Flush().GetAwaiter().GetResult();
        return true;
    }
}