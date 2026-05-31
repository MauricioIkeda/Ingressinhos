using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.TicketReadModel.Dtos;
using IssuedTicketDomain = Ingressinhos.Domain.Sales.Entities.IssuedTicket;

namespace Ingressinhos.Application.Sales.Interfaces;

public interface IUseCaseIssuedTicketCollection : IUseCaseQueryCollection<IssuedTicketDomain>
{
    OperationResult<List<ClientTicketViewDto>> GetMyTickets(int skip = 0, int top = 50);
}
