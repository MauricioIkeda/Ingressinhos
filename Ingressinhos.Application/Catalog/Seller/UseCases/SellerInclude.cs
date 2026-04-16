using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.UseCases;

public class SellerInclude
{
    private readonly IRepositorySession _repositorySession;

    public SellerInclude(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public async Task ExecuteAsync(SellerDto seller)
    {
        if (seller is null)
        {
            throw new Exception("Deve ser informado o vendedor");
        }

        var sellerEntity = new Seller(seller.Name, seller.Email, seller.Cnpj, seller.TradingName)
        {
            CreatedAt = DateTime.Now
        };

        var repository = _repositorySession.GetRepository();
        await repository.IncludeAsync(sellerEntity);
        await repository.FlushAsync();
    }
}
