using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.Interfaces;

public interface IUseCaseTicketCollection : IUseCaseCrudCollection<Ticket, TicketDto>
{
    OperationResult<List<TOutput>> GetQueryItems<TOutput>(Func<IQueryable<TicketQueryItem>, IQueryable<TOutput>> transaction);
}
