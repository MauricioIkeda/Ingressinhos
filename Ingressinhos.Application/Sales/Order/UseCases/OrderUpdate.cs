using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Domain.Sales.Enums;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.UseCases;

public class OrderUpdate : IUseCaseCommand<OrderDto>
{
    public ListMessages Messages { get; } = new();

    private readonly IRepositorySession _repositorySession;

    public OrderUpdate(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(OrderDto orderDto)
    {
        Messages.Clear();

        if (orderDto is null)
        {
            Messages.Add("Deve ser informado o pedido", error: true);
            return false;
        }

        if (orderDto.OrderId <= 0)
        {
            Messages.Add("Deve ser informado o identificador do pedido", error: true);
            return false;
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var orderEntity = repositoryQuery.Return<OrderDomain>(orderDto.OrderId);

            if (orderEntity is null)
            {
                Messages.Add("Pedido nao encontrado", error: true);
                return false;
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
                        Messages.Add("Nao e possivel retornar o pedido para pendente", error: true);
                        return false;
                }
            }

            orderEntity.UpdatedAt = DateTime.UtcNow;

            var repository = _repositorySession.GetRepository();
            repository.Upsert(orderEntity);
            repository.Flush().GetAwaiter().GetResult();
            return true;
        }
        catch (Exception ex)
        {
            Messages.Add(ex);
            return false;
        }
    }
}
