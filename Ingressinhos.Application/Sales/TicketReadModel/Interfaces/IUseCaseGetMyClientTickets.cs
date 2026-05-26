using Generic.Domain.Entities;
using Ingressinhos.Application.Sales.TicketReadModel.Dtos;

namespace Ingressinhos.Application.Sales.TicketReadModel.Interfaces;

public interface IUseCaseGetMyClientTickets
{
    OperationResult<List<ClientTicketViewDto>> Execute();
}
