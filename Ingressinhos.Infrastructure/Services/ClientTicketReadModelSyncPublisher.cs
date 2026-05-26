using Generic.Domain.Entities;
using Generic.Messaging.Contracts;
using Generic.Messaging.Contracts.Tickets;
using Generic.Messaging.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.Interfaces;

namespace Ingressinhos.Infrastructure.Services;

public class ClientTicketReadModelSyncPublisher : IClientTicketReadModelSyncPublisher // Publisher de eventos para sincronizar Mongo com Postgre
{
    private readonly IMessagePublisher _messagePublisher;

    public ClientTicketReadModelSyncPublisher(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public OperationResult RequestOrderTicketsProjection(long orderId)
    {
        return Publish(TicketReadModelSyncKind.OrderTickets, orderId, "Order");
    }

    public OperationResult RequestIssuedTicketProjection(long issuedTicketId)
    {
        return Publish(TicketReadModelSyncKind.IssuedTicket, issuedTicketId, "IssuedTicket");
    }

    public OperationResult RequestEventRefresh(long eventId)
    {
        return Publish(TicketReadModelSyncKind.Event, eventId, "Event");
    }

    public OperationResult RequestLocationRefresh(long locationId)
    {
        return Publish(TicketReadModelSyncKind.Location, locationId, "Location");
    }

    private OperationResult Publish(TicketReadModelSyncKind syncKind, long referenceId, string propertyName)
    {
        if (referenceId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro(propertyName, "Deve ser informado um identificador valido para sincronizar o read model."));
        }

        try
        {
            _messagePublisher.Publish(
                MessageQueues.TicketReadModelSync,
                new TicketReadModelSyncIntegrationEvent(syncKind, referenceId, DateTime.UtcNow));

            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
