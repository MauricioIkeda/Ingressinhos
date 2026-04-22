using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Catalog.Enums;

namespace Ingressinhos.Application.Catalog.UseCases;

public class TicketInclude
{
    private readonly IRepositorySession _repositorySession;

    public TicketInclude(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(TicketDto ticket)
    {
        if (ticket is null)
        {
            throw new Exception("Deve ser informado o ingresso");
        }

        var utcNow = DateTime.UtcNow;

        var littleEvent = _repositorySession.GetRepositoryQuery().Return<Event>(ticket.EventId);
        
        if (littleEvent == null)
        {
            throw new Exception("Evento n�o encontrado");
        }

        var seller = _repositorySession.GetRepositoryQuery().Return<Seller>(ticket.SellerId);

        if (seller != null)
        {
            throw new Exception("O vendedor deve ser informado!");
        }

        if (littleEvent.SellerId != seller.Id)
        {
            throw new Exception("O vendedor do ingresso deve ser dono do evento");
        }
        
        Domain.Catalog.Entities.Location location =  _repositorySession.GetRepositoryQuery().Return<Domain.Catalog.Entities.Location>(littleEvent.LocationId);

        var ticketEntity = new Ticket(
            ticket.EventId,
            ticket.SellerId,
            ticket.Name,
            ticket.BasePrice,
            ticket.PremiumPrice,
            ticket.VipPrice,
            location.TotalCapacity,
            ticket.SalesStartsAt,
            ticket.SalesEndsAt)
        {
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };

        if (!ticket.IsActive)
        {
            ticketEntity.Disable();
        }

        var repository = _repositorySession.GetRepository();
        repository.Include(ticketEntity);
        repository.Flush().GetAwaiter().GetResult();
        return true;
    }
}