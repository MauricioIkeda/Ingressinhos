using Generic.Application.Crud.Interface;
using Ingressinhos.Application.Sales.Dtos;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.Interfaces;

public interface IUseCaseOrderCollection : IUseCaseCrudCollection<OrderDomain, OrderDto>
{
}
