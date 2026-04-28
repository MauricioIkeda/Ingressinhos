using Generic.Application.Crud.Interface;
using Ingressinhos.Application.Sales.Dtos;
using IssuedTicketDomain = Ingressinhos.Domain.Sales.Entities.IssuedTicket;

namespace Ingressinhos.Application.Sales.Interfaces;

public interface IUseCaseIssuedTicketCollection : IUseCaseCrudCollection<IssuedTicketDomain, IssuedTicketDto>
{
}
