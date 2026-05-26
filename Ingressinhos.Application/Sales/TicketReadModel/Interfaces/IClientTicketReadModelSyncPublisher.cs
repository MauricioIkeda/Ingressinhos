using Generic.Domain.Entities;

namespace Ingressinhos.Application.Sales.TicketReadModel.Interfaces;

public interface IClientTicketReadModelSyncPublisher // Interface usada para pedir atualização do ticket no mongo
{
    OperationResult RequestOrderTicketsProjection(long orderId);
    OperationResult RequestIssuedTicketProjection(long issuedTicketId);
    OperationResult RequestEventRefresh(long eventId);
    OperationResult RequestLocationRefresh(long locationId);
}
