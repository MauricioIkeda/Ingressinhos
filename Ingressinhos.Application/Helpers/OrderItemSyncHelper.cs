using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Catalog.Enums;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;
using OrderItemDomain = Ingressinhos.Domain.Sales.Entities.OrderItem;

namespace Ingressinhos.Application.Helpers;

internal static class OrderItemSyncHelper
{
    public static OperationResult Sync(OrderDomain order, IReadOnlyCollection<OrderItemRequest> items, IRepository repository,
        IRepositoryQuery repositoryQuery, DateTime utcNow)
    {
        if (items is null || items.Count == 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Itens", "Informe ao menos um item para o pedido."));
        }

        // O pedido sempre e sincronizado por substituicao total.
        // Primeiro removemos a fotografia atual para depois recriar os itens recebidos.
        var existingItems = order.Items.ToList();

        order.ResetItems();
        if (!order.IsValid)
        {
            return order.ToUnprocessableEntityResult();
        }

        foreach (var existingItem in existingItems)
        {
            repository.Delete(existingItem);
        }

        // Este conjunto impede que o mesmo SeatId entre duas vezes no mesmo request.
        var usedSeatIds = new HashSet<long>();

        foreach (var item in items)
        {
            var buildResult = BuildOrderItem(order.Id, item, repositoryQuery, utcNow, usedSeatIds);
            if (!buildResult.Success)
            {
                return ToOperationResult(buildResult);
            }

            var orderItem = buildResult.Data;

            // O total do pedido continua sendo calculado pelo agregado Order.
            // O OrderItem guarda o detalhamento persistido de cada linha.
            order.AddItem(orderItem.UnitPrice.Value, orderItem.Quantity);
            if (!order.IsValid)
            {
                return order.ToUnprocessableEntityResult();
            }

            repository.Include(orderItem);
        }

        order.UpdatedAt = utcNow;
        repository.Upsert(order);
        return OperationResult.Ok();
    }

    private static OperationResult<OrderItemDomain> BuildOrderItem( long orderId, OrderItemRequest item, IRepositoryQuery repositoryQuery,
        DateTime utcNow, ISet<long>? usedSeatIds)
    {
        if (item is null)
        {
            return OperationResult<OrderItemDomain>.UnprocessableEntity(new MensagemErro("Item", "Envie os dados do item do pedido."));
        }

        var ticket = repositoryQuery.Return<Ticket>(item.TicketId);
        if (ticket is null)
        {
            return OperationResult<OrderItemDomain>.NotFound(new MensagemErro("Ingresso", "Ingresso informado nao foi encontrado."));
        }

        var eventEntity = repositoryQuery.Return<Event>(ticket.EventId);
        if (eventEntity is null)
        {
            return OperationResult<OrderItemDomain>.NotFound(new MensagemErro("Evento", "Evento do ingresso nao foi encontrado."));
        }

        var hasSeatSelection = item.SeatId.HasValue;

        // Eventos com assento geram um OrderItem por assento.
        // Eventos sem assento continuam usando Quantity normalmente.
        if (eventEntity.HasSeats)
        {
            if (!hasSeatSelection)
            {
                return OperationResult<OrderItemDomain>.UnprocessableEntity(new MensagemErro("SeatId", "Eventos com assento exigem um assento por item do pedido."));
            }

            if (item.Quantity > 0 && item.Quantity != 1)
            {
                return OperationResult<OrderItemDomain>.UnprocessableEntity(new MensagemErro("Quantidade", "Itens com assento marcado devem ter quantidade 1."));
            }
        }
        else if (hasSeatSelection)
        {
            return OperationResult<OrderItemDomain>.UnprocessableEntity(new MensagemErro("SeatId", "Este ingresso nao usa assento marcado."));
        }

        var quantity = hasSeatSelection ? 1 : item.Quantity;
        if (quantity <= 0)
        {
            return OperationResult<OrderItemDomain>.UnprocessableEntity(new MensagemErro("Quantidade", "A quantidade precisa ser maior que zero."));
        }

        Seat? seatEntity = null;
        var category = SeatCategory.Standard;

        if (hasSeatSelection)
        {
            seatEntity = repositoryQuery.Return<Seat>(item.SeatId!.Value);
            if (seatEntity is null)
            {
                return OperationResult<OrderItemDomain>.NotFound(new MensagemErro("SeatId", "Assento informado nao foi encontrado."));
            }

            if (seatEntity.LocationId != eventEntity.LocationId)
            {
                return OperationResult<OrderItemDomain>.UnprocessableEntity(new MensagemErro("SeatId", "O assento informado nao pertence ao local do evento."));
            }

            if (usedSeatIds is not null && !usedSeatIds.Add(seatEntity.Id))
            {
                return OperationResult<OrderItemDomain>.UnprocessableEntity(new MensagemErro("SeatId", "O mesmo assento nao pode ser informado duas vezes no pedido."));
            }

            // O preco do item pode variar conforme a categoria do assento escolhido.
            category = seatEntity.Category;
        }

        var orderItemEntity = new OrderItemDomain(
            orderId,
            ticket.Id,
            ticket.Name,
            quantity,
            ResolveUnitPrice(ticket, category),
            category,
            seatEntity?.Id,
            seatEntity?.Code)
        {
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };

        if (!orderItemEntity.IsValid)
        {
            return OperationResult<OrderItemDomain>.FromResult(orderItemEntity.ToUnprocessableEntityResult());
        }

        return OperationResult<OrderItemDomain>.Created(orderItemEntity);
    }

    private static OperationResult ToOperationResult(OperationResult<OrderItemDomain> result)
    {
        return result.StatusCode switch
        {
            401 => OperationResult.Unauthorized(result.Errors.First()),
            403 => OperationResult.Forbidden(result.Errors.First()),
            404 => OperationResult.NotFound(result.Errors.First()),
            422 => OperationResult.UnprocessableEntity(result.Errors),
            500 => OperationResult.FatalError(result.Errors.First()),
            _ => OperationResult.Fail(result.Errors)
        };
    }

    private static decimal ResolveUnitPrice(Ticket ticket, SeatCategory category)
    {
        return category switch
        {
            SeatCategory.Premium when ticket.PremiumPrice is not null => ticket.PremiumPrice.Value,
            SeatCategory.Vip when ticket.VIPPrice is not null => ticket.VIPPrice.Value,
            _ => ticket.BasePrice.Value
        };
    }
}
