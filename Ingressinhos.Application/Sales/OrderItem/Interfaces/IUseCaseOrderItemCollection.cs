using Generic.Application.Crud.Interface;
using OrderItemDomain = Ingressinhos.Domain.Sales.Entities.OrderItem;

namespace Ingressinhos.Application.Sales.Interfaces;

public interface IUseCaseOrderItemCollection : IUseCaseQueryCollection<OrderItemDomain>
{
}
