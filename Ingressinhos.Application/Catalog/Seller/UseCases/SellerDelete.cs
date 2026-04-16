using Generic.Infrastructure.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.UseCases;

public class SellerDelete
{
    private readonly IRepositorySession _repositorySession;

    public SellerDelete(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public async Task<bool> ExecuteAsync(long sellerId)
    {
        var seller = _repositorySession.GetRepositoryQuery().Return<Seller>(sellerId);

        if (seller is null)
        {
            throw new Exception("Vendedor nao encontrado");
        }

        try
        {
            var repository = _repositorySession.GetRepository();
            await repository.DeleteAsync(seller);
            await repository.FlushAsync();
            return true;
        }
        catch (Exception)
        {
            throw new Exception("Erro ao deletar o vendedor");
        }
    }
}