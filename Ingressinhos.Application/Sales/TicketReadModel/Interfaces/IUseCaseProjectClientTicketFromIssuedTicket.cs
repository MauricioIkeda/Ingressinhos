using Generic.Domain.Entities;

namespace Ingressinhos.Application.Sales.TicketReadModel.Interfaces;

public interface IUseCaseProjectClientTicketFromIssuedTicket
{
    OperationResult Execute(long issuedTicketId);
}
