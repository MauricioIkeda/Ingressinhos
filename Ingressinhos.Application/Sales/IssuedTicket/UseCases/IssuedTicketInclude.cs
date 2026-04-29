using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Sales.Entities;
using IssuedTicketDomain = Ingressinhos.Domain.Sales.Entities.IssuedTicket;

namespace Ingressinhos.Application.Sales.UseCases;

public class IssuedTicketInclude : IUseCaseCommand<IssuedTicketDto>
{
    public ListMessages Messages { get; } = new();

    private readonly IRepositorySession _repositorySession;

    public IssuedTicketInclude(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(IssuedTicketDto issuedTicketDto)
    {
        Messages.Clear();

        if (issuedTicketDto is null)
        {
            Messages.Add("Deve ser informado o ingresso emitido", error: true);
            return false;
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();

            if (repositoryQuery.Return<OrderItem>(issuedTicketDto.OrderItemId) is null)
            {
                Messages.Add("Deve ser informado um pedido valido", error: true);
                return false;
            }
            
            if (repositoryQuery.Return<Client>(issuedTicketDto.ClientId) is null)
            {
                Messages.Add("Deve ser informado um cliente valido", error: true);
                return false;
            }
            
            if (repositoryQuery.Return<Event>(issuedTicketDto.EventId) is null)
            {
                Messages.Add("Deve ser informado um evento valido", error: true);
                return false;
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
        catch (Exception ex)
        {
            Messages.Add(ex);
            return false;
        }
    }
}
