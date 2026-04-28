using Generic.Application.Crud.Interface;
using Ingressinhos.Application.Sales.Dtos;
using OrderItemDomain = Ingressinhos.Domain.Sales.Entities.OrderItem;

namespace Ingressinhos.Application.Sales.Interfaces;

public interface IUseCaseOrderItemCollection : IUseCaseCrudCollection<OrderItemDomain, OrderItemDto>
{
}
