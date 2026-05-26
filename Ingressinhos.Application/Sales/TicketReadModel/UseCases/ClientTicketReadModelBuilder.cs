using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.Models;
using Ingressinhos.Domain.Sales.Entities;
using EventDomain = Ingressinhos.Domain.Catalog.Entities.Event;
using LocationDomain = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Sales.TicketReadModel.UseCases;

public class ClientTicketReadModelBuilder
{
    public OperationResult<ClientTicketReadModelEntry> Build(long issuedTicketId, IRepositoryQuery repositoryQuery)
    {
        var issuedTicket = repositoryQuery.Return<IssuedTicket>(issuedTicketId);
        if (issuedTicket is null)
        {
            return OperationResult<ClientTicketReadModelEntry>.NotFound(new MensagemErro("IssuedTicket", "Bilhete emitido nao encontrado."));
        }

        var orderItem = repositoryQuery.Return<OrderItem>(issuedTicket.OrderItemId);
        if (orderItem is null)
        {
            return OperationResult<ClientTicketReadModelEntry>.NotFound(new MensagemErro("OrderItem", "Item do pedido do bilhete nao encontrado."));
        }

        var order = repositoryQuery.Return<Order>(orderItem.OrderId);
        if (order is null)
        {
            return OperationResult<ClientTicketReadModelEntry>.NotFound(new MensagemErro("Order", "Pedido do bilhete nao encontrado."));
        }

        var client = repositoryQuery.Return<Client>(issuedTicket.ClientId);
        if (client is null)
        {
            return OperationResult<ClientTicketReadModelEntry>.NotFound(new MensagemErro("Client", "Cliente do bilhete nao encontrado."));
        }

        var eventEntity = repositoryQuery.Return<EventDomain>(issuedTicket.EventId);
        if (eventEntity is null)
        {
            return OperationResult<ClientTicketReadModelEntry>.NotFound(new MensagemErro("Event", "Evento do bilhete nao encontrado."));
        }

        var location = repositoryQuery.Return<LocationDomain>(eventEntity.LocationId);
        if (location is null)
        {
            return OperationResult<ClientTicketReadModelEntry>.NotFound(new MensagemErro("Location", "Local do evento nao encontrado."));
        }

        return OperationResult<ClientTicketReadModelEntry>.Ok(new ClientTicketReadModelEntry
        {
            IssuedTicketId = issuedTicket.Id,
            AccessCode = issuedTicket.AccessCode,
            Status = issuedTicket.Status.ToString(),
            IssuedAtUtc = issuedTicket.IssuedAt,
            CheckedInAtUtc = issuedTicket.CheckedInAt,
            CancelledAtUtc = issuedTicket.CancelledAt,
            PaidAtUtc = order.PaidAt,
            ClientId = client.Id,
            ClientUserId = client.UserId,
            OrderId = order.Id,
            OrderItemId = orderItem.Id,
            TicketName = orderItem.TicketName,
            SeatCode = orderItem.SeatCode,
            Category = orderItem.Category.ToString(),
            EventId = eventEntity.Id,
            EventName = eventEntity.Name,
            EventStartTimeUtc = eventEntity.StartTime,
            EventEndTimeUtc = eventEntity.EndTime,
            EventImageUrl = eventEntity.ImageUrl,
            LocationId = location.Id,
            LocationName = location.Name,
            ProjectedAtUtc = DateTime.UtcNow
        });
    }
}
