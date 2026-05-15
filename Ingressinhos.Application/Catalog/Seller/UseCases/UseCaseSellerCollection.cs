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
    private readonly SellerGetByToken _sellerGetByToken;
    private readonly SellerGetOdata _sellerGetOdata;

    public UseCaseSellerCollection(
        IRepositorySession repositorySession,
        SellerUpdate update,
        SellerInclude sellerInclude,
        SellerDeactivate sellerDeactivate,
        SellerRecover sellerRecover,
        SellerGetByToken sellerGetByToken,
        SellerGetOdata sellerGetOdata)
        : base(sellerInclude, update, new UseCaseGetOdata<Seller>(), new UseCaseGetId<Seller>(), new UseCaseDelete<Seller>(), repositorySession)
    {
        _sellerDeactivate = sellerDeactivate;
        _sellerRecover = sellerRecover;
        _sellerGetByToken = sellerGetByToken;
        _sellerGetOdata = sellerGetOdata;
    }

    public OperationResult Deactivate(long id)
    {
        return _sellerDeactivate.Execute(id);
    }

    public OperationResult Recover(long id)
    {
        return _sellerRecover.Execute(id);
    }

    public OperationResult<SellerGet> GetByToken()
    {
        return _sellerGetByToken.Execute();
    }

    public OperationResult<List<TOutput>> GetQueryItems<TOutput>(Func<IQueryable<SellerQueryItem>, IQueryable<TOutput>> transaction)
    {
        return _sellerGetOdata.Execute(transaction);
    }
}
