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
        if (_repositorySession.GetRepositoryQuery().Return<Event>(ticket.EventId) == null)
        {
            throw new Exception("Evento não encontrado");
        }

        var ticketEntity = new Ticket(
            ticket.EventId,
            ticket.Name,
            ticket.BasePrice,
            ticket.PremiumPrice,
            ticket.VipPrice,
            ticket.TotalQuantity,
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