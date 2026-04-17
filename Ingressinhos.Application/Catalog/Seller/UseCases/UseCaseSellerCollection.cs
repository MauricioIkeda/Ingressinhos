using Generic.Application.UseCases;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Application.Catalog.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.UseCases;

public class UseCaseSellerCollection : UseCaseCrudCollection<Seller, SellerDto>, IUseCaseSellerCollection
{
    public UseCaseSellerCollection(IRepositorySession repositorySession, SellerUpdate update, SellerInclude sellerInclude)
        : base( sellerInclude.Execute, update.Execute, new UseCaseGetOdata<Seller>(), new UseCaseGet<Seller>(), new UseCaseDelete<Seller>(), repositorySession)
    {
    }
}
