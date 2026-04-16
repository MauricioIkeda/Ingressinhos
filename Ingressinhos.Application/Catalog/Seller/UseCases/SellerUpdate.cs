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

    public async Task ExecuteAsync(SellerDto seller)
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

        sellerEntity.ChangeName(seller.Name);
        sellerEntity.ChangeEmail(seller.Email);
        sellerEntity.ChangeTradingName(seller.TradingName);

        var repository = _repositorySession.GetRepository();
        await repository.UpsertAsync(sellerEntity);
        await repository.FlushAsync();
    }
}