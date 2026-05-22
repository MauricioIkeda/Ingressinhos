using Generic.Application.Crud.Interface;
using Generic.Application.Dtos;
using Generic.Domain.Entities;
using Ingressinhos.Application.Sales.Dtos;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.Interfaces;

public interface IUseCaseOrderCollection : IUseCaseQueryCollection<OrderDomain>
{
    OperationResult<OrderDomain> GetCurrentCart(long clientId = 0);
    OperationResult AddCartItem(AddCartItemRequest command);
    OperationResult RemoveCartItem(long orderItemId);
    OperationResult ResetCart(long clientId = 0);
    OperationResult<PaymentCheckoutApiDto> Close(long orderId);
    OperationResult<PaymentCheckoutApiDto> Immediate(CreateOrderRequest command);
}
