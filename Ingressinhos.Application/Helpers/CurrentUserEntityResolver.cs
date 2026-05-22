using Generic.Application.Utils.Interface;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;
using ClientDomain = Ingressinhos.Domain.Sales.Entities.Client;

namespace Ingressinhos.Application.Helpers;

internal static class CurrentUserEntityResolver // Entidade que encontra o cliente e o vendedor pelo token, ou via id se for adm
{
    public static ClientDomain? ResolveClient(ICurrentUserContext currentUserContext, IRepositoryQuery repositoryQuery, long clientId = 0, bool onlyActive = true)
    {
        if (currentUserContext.Role == "Admin")
        {
            if (clientId <= 0)
            {
                return null;
            }

            return onlyActive
                ? repositoryQuery.Query<ClientDomain>(client => client.Id == clientId && client.Active).FirstOrDefault()
                : repositoryQuery.Return<ClientDomain>(clientId);
        }

        return onlyActive
            ? repositoryQuery.Query<ClientDomain>(client => client.UserId == currentUserContext.UserId && client.Active).FirstOrDefault()
            : repositoryQuery.Query<ClientDomain>(client => client.UserId == currentUserContext.UserId).FirstOrDefault();
    }

    public static Seller? ResolveSeller(ICurrentUserContext currentUserContext, IRepositoryQuery repositoryQuery, long sellerId = 0, bool onlyActive = true)
    {
        if (currentUserContext.Role == "Admin")
        {
            if (sellerId <= 0)
            {
                return null;
            }

            return onlyActive
                ? repositoryQuery.Query<Seller>(seller => seller.Id == sellerId && seller.Active).FirstOrDefault()
                : repositoryQuery.Return<Seller>(sellerId);
        }

        return onlyActive
            ? repositoryQuery.Query<Seller>(seller => seller.UserId == currentUserContext.UserId && seller.Active).FirstOrDefault()
            : repositoryQuery.Query<Seller>(seller => seller.UserId == currentUserContext.UserId).FirstOrDefault();
    }
}
