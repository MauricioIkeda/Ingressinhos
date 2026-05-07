using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.Interfaces
{
    public interface IUseCaseSellerCollection : IUseCaseCrudCollection<Seller, SellerDto>
    {
        OperationResult Deactivate(long id);
        OperationResult Recover(long id);
    }
}
