using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Sales.Entities;
using IssuedTicketDomain = Ingressinhos.Domain.Sales.Entities.IssuedTicket;

namespace Ingressinhos.Application.Sales.UseCases;

public class IssuedTicketInclude
{
    private readonly IRepositorySession _repositorySession;

    public IssuedTicketInclude(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(IssuedTicketDto issuedTicketDto)
    {
        if (issuedTicketDto is null)
        {
            throw new Exception("Deve ser informado o ingresso emitido");
        }

        if (_repositorySession.GetRepositoryQuery().Return<OrderItem>(issuedTicketDto.OrderItemId) is null)
        {
            throw new Exception("Deve ser informado um pedido valido");
        }
        
        if (_repositorySession.GetRepositoryQuery().Return<Client>(issuedTicketDto.ClientId) is null)
        {
            throw new Exception("Deve ser informado um cliente valido");
        }
        
        if (_repositorySession.GetRepositoryQuery().Return<Event>(issuedTicketDto.EventId) is null)
        {
            throw new Exception("Deve ser informado um evento valido");
        }

        var utcNow = DateTime.UtcNow;
        
        var issuedTicketEntity = new IssuedTicketDomain(
            issuedTicketDto.OrderItemId,
            issuedTicketDto.ClientId,
            issuedTicketDto.EventId,
            issuedTicketDto.AccessCode)
        {
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };

        var repository = _repositorySession.GetRepository();
        repository.Include(issuedTicketEntity);
        repository.Flush().GetAwaiter().GetResult();
        return true;
    }
}
