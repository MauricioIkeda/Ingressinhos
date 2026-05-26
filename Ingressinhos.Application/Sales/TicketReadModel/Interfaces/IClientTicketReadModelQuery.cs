using Ingressinhos.Application.Sales.TicketReadModel.Models;

namespace Ingressinhos.Application.Sales.TicketReadModel.Interfaces;

public interface IClientTicketReadModelQuery // Query do Mongo DB
{
    IReadOnlyCollection<ClientTicketReadModelEntry> GetByClientUserId(string clientUserId);
}
