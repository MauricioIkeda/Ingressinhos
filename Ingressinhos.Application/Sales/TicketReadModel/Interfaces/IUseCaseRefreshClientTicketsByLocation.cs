using Generic.Domain.Entities;

namespace Ingressinhos.Application.Sales.TicketReadModel.Interfaces;

public interface IUseCaseRefreshClientTicketsByLocation
{
    OperationResult Execute(long locationId);
}
