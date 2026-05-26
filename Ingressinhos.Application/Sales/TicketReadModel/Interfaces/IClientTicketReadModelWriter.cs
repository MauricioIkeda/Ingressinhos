using Ingressinhos.Application.Sales.TicketReadModel.Models;

namespace Ingressinhos.Application.Sales.TicketReadModel.Interfaces;

public interface IClientTicketReadModelWriter // Inserir ou atualizar do Mongo DB
{
    void Upsert(ClientTicketReadModelEntry ticket);
    void UpsertMany(IReadOnlyCollection<ClientTicketReadModelEntry> tickets);
}
