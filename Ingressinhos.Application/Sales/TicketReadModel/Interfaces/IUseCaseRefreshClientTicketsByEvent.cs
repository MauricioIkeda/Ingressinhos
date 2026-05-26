using Generic.Domain.Entities;

namespace Ingressinhos.Application.Sales.TicketReadModel.Interfaces;

public interface IUseCaseRefreshClientTicketsByEvent
{
    OperationResult Execute(long eventId);
}
