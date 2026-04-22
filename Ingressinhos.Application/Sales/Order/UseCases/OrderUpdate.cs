using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Domain.Sales.Enums;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.UseCases;

public class OrderUpdate
{
    private readonly IRepositorySession _repositorySession;

    public OrderUpdate(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(OrderDto orderDto)
    {
        if (orderDto is null)
        {
            throw new Exception("Deve ser informado o pedido");
        }

        if (orderDto.OrderId <= 0)
        {
            throw new Exception("Deve ser informado o identificador do pedido");
        }

        var repositoryQuery = _repositorySession.GetRepositoryQuery();
        var orderEntity = repositoryQuery.Return<OrderDomain>(orderDto.OrderId);

        if (orderEntity is null)
        {
            throw new Exception("Pedido nao encontrado");
        }

        if (orderDto.Status != orderEntity.Status)
        {
            switch (orderDto.Status)
            {
                case OrderStatus.Paid:
                    orderEntity.ConfirmPayment();
                    break;
                case OrderStatus.Cancelled:
                    orderEntity.Cancel();
                    break;
                default:
                    throw new Exception("Nao eh possivel retornar o pedido para pendente");
            }
        }

        orderEntity.UpdatedAt = DateTime.UtcNow;

        var repository = _repositorySession.GetRepository();
        repository.Upsert(orderEntity);
        repository.Flush().GetAwaiter().GetResult();
        return true;
    }
}
