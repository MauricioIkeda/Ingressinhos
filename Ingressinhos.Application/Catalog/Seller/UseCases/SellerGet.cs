using Generic.Infrastructure.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.UseCases;

public class SellerGet
{
    private readonly IRepositorySession _repositorySession;

    public SellerGet(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public Seller Execute(long sellerId)
    {
        if (sellerId <= 0)
        {
            throw new Exception("Deve ser informado o identificador do vendedor");
        }

        var seller = _repositorySession.GetRepositoryQuery().Return<Seller>(sellerId);

        if (seller is null)
        {
            throw new Exception("Vendedor nao encontrado");
        }

        return seller;
    }
}