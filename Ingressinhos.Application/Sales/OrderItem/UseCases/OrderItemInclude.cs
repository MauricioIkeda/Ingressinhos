using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using OrderItemDomain = Ingressinhos.Domain.Sales.Entities.OrderItem;

namespace Ingressinhos.Application.Sales.UseCases;

public class OrderItemInclude : IUseCaseCommand<OrderItemDto>
{
    private readonly IRepositorySession _repositorySession;

    public OrderItemInclude(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult Execute(OrderItemDto orderItemDto)
    {
        if (orderItemDto is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("OrderItem", "Deve ser informado o item do pedido."));
        }

        try
        {
            var utcNow = DateTime.UtcNow;

            var orderItemEntity = new OrderItemDomain(
                orderItemDto.OrderId,
                orderItemDto.TicketId,
                orderItemDto.TicketName,
                orderItemDto.Quantity,
                orderItemDto.UnitPrice)
            {
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };

            var repository = _repositorySession.GetRepository();
            repository.Include(orderItemEntity);
            repository.Flush().GetAwaiter().GetResult();
            return OperationResult.Created();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
