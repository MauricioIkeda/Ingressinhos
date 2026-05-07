using Generic.Application.Crud.UseCases;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Application.Catalog.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.UseCases;

public class UseCaseSellerCollection : UseCaseCrudCollection<Seller, SellerDto>, IUseCaseSellerCollection
{
    private readonly SellerDeactivate _sellerDeactivate;
    private readonly SellerRecover _sellerRecover;

    public UseCaseSellerCollection(
        IRepositorySession repositorySession,
        SellerUpdate update,
        SellerInclude sellerInclude,
        SellerDeactivate sellerDeactivate,
        SellerRecover sellerRecover)
        : base(sellerInclude, update, new UseCaseGetOdata<Seller>(), new UseCaseGet<Seller>(), new UseCaseDelete<Seller>(), repositorySession)
    {
        _sellerDeactivate = sellerDeactivate;
        _sellerRecover = sellerRecover;
    }

    public OperationResult Deactivate(long id)
    {
        return _sellerDeactivate.Execute(id);
    }

    public OperationResult Recover(long id)
    {
        return _sellerRecover.Execute(id);
    }
}
