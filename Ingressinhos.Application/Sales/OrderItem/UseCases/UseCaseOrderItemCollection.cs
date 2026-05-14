using Generic.Application.Crud.UseCases;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Interfaces;
using OrderItemDomain = Ingressinhos.Domain.Sales.Entities.OrderItem;

namespace Ingressinhos.Application.Sales.UseCases;

public class UseCaseOrderItemCollection : UseCaseQueryCollection<OrderItemDomain>, IUseCaseOrderItemCollection
{
    public UseCaseOrderItemCollection(IRepositorySession repositorySession)
        : base(new UseCaseGetOdata<OrderItemDomain>(), new UseCaseGetId<OrderItemDomain>(), repositorySession)
    {
    }
}
