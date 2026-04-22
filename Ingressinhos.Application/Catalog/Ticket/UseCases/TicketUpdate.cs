using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Catalog.Enums;

namespace Ingressinhos.Application.Catalog.UseCases;

public class TicketUpdate
{
    private readonly IRepositorySession _repositorySession;

    public TicketUpdate(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(TicketDto ticket)
    {
        if (ticket is null)
        {
            throw new Exception("Deve ser informado o ingresso");
        }

        if (ticket.TicketId <= 0)
        {
            throw new Exception("Deve ser informado o identificador do ingresso");
        }

        var repositoryQuery = _repositorySession.GetRepositoryQuery();
        var ticketEntity = repositoryQuery.Return<Ticket>(ticket.TicketId);

        if (ticketEntity is null)
        {
            throw new Exception("Ingresso nao encontrado");
        }

        if (ticket.BasePrice != ticketEntity.BasePrice.Value ||
            ticket.PremiumPrice != ticketEntity.PremiumPrice?.Value ||
            ticket.VipPrice != ticketEntity.VIPPrice?.Value)
        {
            ticketEntity.ChangePrices(ticket.BasePrice, ticket.PremiumPrice, ticket.VipPrice);
        }

        if (ticket.TotalQuantity > ticketEntity.TotalQuantity)
        {
            ticketEntity.AddCapacity(ticket.TotalQuantity - ticketEntity.TotalQuantity);
        }

        if (ticket.IsActive && ticketEntity.Status == CatalogTicketStatus.Inactive)
        {
            ticketEntity.Enable();
        }

        if (!ticket.IsActive && ticketEntity.Status != CatalogTicketStatus.Inactive)
        {
            ticketEntity.Disable();
        }

        ticketEntity.UpdatedAt = DateTime.UtcNow;

        var repository = _repositorySession.GetRepository();
        repository.Upsert(ticketEntity);
        repository.Flush().GetAwaiter().GetResult();
        return true;
    }
}