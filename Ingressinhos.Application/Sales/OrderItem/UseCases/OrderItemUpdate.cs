using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using OrderItemDomain = Ingressinhos.Domain.Sales.Entities.OrderItem;

namespace Ingressinhos.Application.Sales.UseCases;

public class OrderItemUpdate : IUseCaseCommand<OrderItemDto>
{
    private readonly IRepositorySession _repositorySession;

    public OrderItemUpdate(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult Execute(OrderItemDto orderItemDto)
    {
        if (orderItemDto is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("OrderItem", "Deve ser informado o item do pedido."));
        }

        if (orderItemDto.OrderItemId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Id", "Deve ser informado o identificador do item do pedido."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var orderItemEntity = repositoryQuery.Return<OrderItemDomain>(orderItemDto.OrderItemId);

            if (orderItemEntity is null)
            {
                return OperationResult.NotFound(new MensagemErro("Id", "Item do pedido nao encontrado."));
            }

            if (orderItemDto.OrderId != orderItemEntity.OrderId ||
                orderItemDto.TicketId != orderItemEntity.TicketId ||
                orderItemDto.TicketName != orderItemEntity.TicketName ||
                orderItemDto.Quantity != orderItemEntity.Quantity ||
                orderItemDto.UnitPrice != orderItemEntity.UnitPrice.Value)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("OrderItem", "Nao eh permitido alterar um item de pedido existente. Exclua e inclua novamente."));
            }

            orderItemEntity.UpdatedAt = DateTime.UtcNow;

            var repository = _repositorySession.GetRepository();
            repository.Upsert(orderItemEntity);
            repository.Flush().GetAwaiter().GetResult();
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
