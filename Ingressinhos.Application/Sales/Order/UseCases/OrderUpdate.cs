using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Domain.Sales.Enums;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.UseCases;

public class OrderUpdate : IUseCaseCommand<OrderDto>
{
    private readonly IRepositorySession _repositorySession;

    public OrderUpdate(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult Execute(OrderDto orderDto)
    {
        if (orderDto is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Order", "Deve ser informado o pedido."));
        }

        if (orderDto.OrderId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Id", "Deve ser informado o identificador do pedido."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var orderEntity = repositoryQuery.Return<OrderDomain>(orderDto.OrderId);

            if (orderEntity is null)
            {
                return OperationResult.NotFound(new MensagemErro("Id", "Pedido nao encontrado."));
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
                        return OperationResult.UnprocessableEntity(new MensagemErro("Status", "Nao e possivel retornar o pedido para pendente."));
                }
            }

            orderEntity.UpdatedAt = DateTime.UtcNow;

            var repository = _repositorySession.GetRepository();
            repository.Upsert(orderEntity);
            repository.Flush().GetAwaiter().GetResult();
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
