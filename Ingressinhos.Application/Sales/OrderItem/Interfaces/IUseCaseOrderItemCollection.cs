using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Ingressinhos.Application.Sales.Dtos;
using OrderItemDomain = Ingressinhos.Domain.Sales.Entities.OrderItem;

namespace Ingressinhos.Application.Sales.Interfaces;

public interface IUseCaseOrderItemCollection : IUseCaseQueryCollection<OrderItemDomain>
{
    OperationResult<List<TOutput>> GetQueryItems<TOutput>(Func<IQueryable<OrderItemQueryItem>, IQueryable<TOutput>> transaction);
}
