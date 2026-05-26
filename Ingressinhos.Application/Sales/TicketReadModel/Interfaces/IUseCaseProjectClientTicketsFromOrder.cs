using Generic.Domain.Entities;

namespace Ingressinhos.Application.Sales.TicketReadModel.Interfaces;

public interface IUseCaseProjectClientTicketsFromOrder
{
    OperationResult Execute(long orderId);
}
