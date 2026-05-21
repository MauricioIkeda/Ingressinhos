using Generic.Application.Crud.Interface;
using Generic.Application.Dtos;
using Generic.Domain.Entities;
using Ingressinhos.Application.Sales.Dtos;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.Interfaces;

public interface IUseCaseOrderCollection : IUseCaseQueryCollection<OrderDomain>
{
    OperationResult Create(CreateOrderRequest command);
    OperationResult UpdateItems(UpdateOrderItemsRequest command);
    OperationResult Delete(long id);
    OperationResult<PaymentCheckoutApiDto> Close(long orderId);
    OperationResult<PaymentCheckoutApiDto> Immediate(CreateOrderRequest command);
}
