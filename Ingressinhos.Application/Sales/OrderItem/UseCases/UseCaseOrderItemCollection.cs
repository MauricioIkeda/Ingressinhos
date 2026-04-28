using Generic.Application.Crud.UseCases;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.Interfaces;
using OrderItemDomain = Ingressinhos.Domain.Sales.Entities.OrderItem;

namespace Ingressinhos.Application.Sales.UseCases;

public class UseCaseOrderItemCollection : UseCaseCrudCollection<OrderItemDomain, OrderItemDto>, IUseCaseOrderItemCollection
{
    public UseCaseOrderItemCollection(IRepositorySession repositorySession, OrderItemUpdate update, OrderItemInclude include)
        : base(include.Execute, update.Execute, new UseCaseGetOdata<OrderItemDomain>(), new UseCaseGet<OrderItemDomain>(), new UseCaseDelete<OrderItemDomain>(), repositorySession)
    {
    }
}
