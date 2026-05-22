using Generic.Application.Crud.UseCases;
using Generic.Application.Dtos;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.Interfaces;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.UseCases;

public class UseCaseOrderCollection : UseCaseQueryCollection<OrderDomain>, IUseCaseOrderCollection
{
    private readonly GetCurrentCart _getCurrentCart;
    private readonly AddCartItem _addCartItem;
    private readonly RemoveCartItem _removeCartItem;
    private readonly ResetCart _resetCart;
    private readonly IUseCaseCloseOrder _closeOrder;
    private readonly IUseCaseImmediateOrder _immediateOrder;

    public UseCaseOrderCollection(
        IRepositorySession repositorySession,
        IReadRepositoryQuery readRepositoryQuery,
        GetCurrentCart getCurrentCart,
        AddCartItem addCartItem,
        RemoveCartItem removeCartItem,
        ResetCart resetCart,
        IUseCaseCloseOrder closeOrder,
        IUseCaseImmediateOrder immediateOrder)
        : base(new UseCaseGetOdata<OrderDomain>(), new UseCaseGetId<OrderDomain>(), repositorySession, readRepositoryQuery)
    {
        _getCurrentCart = getCurrentCart;
        _addCartItem = addCartItem;
        _removeCartItem = removeCartItem;
        _resetCart = resetCart;
        _closeOrder = closeOrder;
        _immediateOrder = immediateOrder;
    }

    public OperationResult<OrderDomain> GetCurrentCart(long clientId = 0)
    {
        return _getCurrentCart.Execute(clientId);
    }

    public OperationResult AddCartItem(AddCartItemRequest command)
    {
        return _addCartItem.Execute(command);
    }

    public OperationResult RemoveCartItem(long orderItemId)
    {
        return _removeCartItem.Execute(orderItemId);
    }

    public OperationResult ResetCart(long clientId = 0)
    {
        return _resetCart.Execute(clientId);
    }

    public OperationResult<PaymentCheckoutApiDto> Close(long orderId)
    {
        return _closeOrder.Execute(orderId);
    }

    public OperationResult<PaymentCheckoutApiDto> Immediate(CreateOrderRequest command)
    {
        return _immediateOrder.Execute(command);
    }
}
