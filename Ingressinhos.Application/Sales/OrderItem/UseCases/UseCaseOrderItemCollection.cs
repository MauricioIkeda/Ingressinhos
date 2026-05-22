using Generic.Application.Crud.UseCases;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.Interfaces;
using OrderItemDomain = Ingressinhos.Domain.Sales.Entities.OrderItem;

namespace Ingressinhos.Application.Sales.UseCases;

public class UseCaseOrderItemCollection : UseCaseQueryCollection<OrderItemDomain>, IUseCaseOrderItemCollection
{
    private readonly OrderItemGetOdata _orderItemGetOdata;

    public UseCaseOrderItemCollection(IRepositorySession repositorySession, IReadRepositoryQuery readRepositoryQuery, OrderItemGetOdata orderItemGetOdata)
        : base(new UseCaseGetOdata<OrderItemDomain>(), new UseCaseGetId<OrderItemDomain>(), repositorySession, readRepositoryQuery)
    {
        _orderItemGetOdata = orderItemGetOdata;
    }

    public OperationResult<List<TOutput>> GetQueryItems<TOutput>(Func<IQueryable<OrderItemQueryItem>, IQueryable<TOutput>> transaction)
    {
        return _orderItemGetOdata.Execute(transaction);
    }
}
