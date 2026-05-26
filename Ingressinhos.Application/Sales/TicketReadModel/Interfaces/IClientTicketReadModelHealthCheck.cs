namespace Ingressinhos.Application.Sales.TicketReadModel.Interfaces;

public interface IClientTicketReadModelHealthCheck // Verifica a conexão do Mongo
{
    bool IsAvailable();
}
