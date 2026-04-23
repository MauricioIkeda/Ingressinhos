using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Catalog.Enums;
using LocationDoman = Ingressinhos.Domain.Catalog.Entities.Location;
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
        IRepositoryQuery repositoryQuery = _repositorySession.GetRepositoryQuery();

        var littleEvent = repositoryQuery.Return<Event>(ticket.EventId);
        
        if (littleEvent == null)
        {
            throw new Exception("Evento não encontrado");
        }

        Seller seller = repositoryQuery.Return<Seller>(ticket.SellerId);

        if (seller == null)
        {
            throw new Exception("O vendedor deve ser informado!");
        }

        if (littleEvent.SellerId != seller.Id)
        {
            throw new Exception("O vendedor do ingresso deve ser dono do evento");
        }
        
        LocationDoman location = repositoryQuery.Return<LocationDoman>(littleEvent.LocationId);
        if (location is null)
        {
            throw new Exception("Local do evento nao encontrado");
        }

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