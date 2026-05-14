using Generic.Application.Crud.Interface;
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
    private readonly CreateOrder _createOrder;
    private readonly UpdateOrderItems _updateOrderItems;
    private readonly IUseCaseCloseOrder _closeOrder;
    private readonly IUseCaseDelete<OrderDomain> _deleteOrder;

    public UseCaseOrderCollection(
        IRepositorySession repositorySession,
        CreateOrder createOrder,
        UpdateOrderItems updateOrderItems,
        IUseCaseCloseOrder closeOrder,
        IUseCaseDelete<OrderDomain> deleteOrder)
        : base(new UseCaseGetOdata<OrderDomain>(), new UseCaseGetId<OrderDomain>(), repositorySession)
    {
        _createOrder = createOrder;
        _updateOrderItems = updateOrderItems;
        _closeOrder = closeOrder;
        _deleteOrder = deleteOrder;
    }

    public OperationResult Create(CreateOrderRequest command)
    {
        return _createOrder.Execute(command);
    }

    public OperationResult UpdateItems(UpdateOrderItemsRequest command)
    {
        return _updateOrderItems.Execute(command);
    }

    public OperationResult Delete(long id)
    {
        return _deleteOrder.Execute(id, _repositorySession);
    }

    public OperationResult<PaymentCheckoutApiDto> Close(long orderId)
    {
        return _closeOrder.Execute(orderId);
    }
}
