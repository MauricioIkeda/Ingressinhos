using Generic.Application.UseCases;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.Interfaces;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.UseCases;

public class UseCaseOrderCollection : UseCaseCrudCollection<OrderDomain, OrderDto>, IUseCaseOrderCollection
{
    public UseCaseOrderCollection(IRepositorySession repositorySession, OrderUpdate update, OrderInclude include)
        : base(include.Execute, update.Execute, new UseCaseGetOdata<OrderDomain>(), new UseCaseGet<OrderDomain>(), new UseCaseDelete<OrderDomain>(), repositorySession)
    {
    }
}
