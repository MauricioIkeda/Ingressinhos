using Generic.Application.Crud.UseCases;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.UseCases;

public class TicketGetOdata : UseCaseGetQueryItems<Ticket, TicketQueryItem>
{
    public TicketGetOdata(IReadRepositoryQuery readRepositoryQuery)
        : base(readRepositoryQuery)
    {
    }

    protected override TicketQueryItem ToQueryItem(Ticket ticket)
    {
        return new TicketQueryItem
        {
            Id = ticket.Id,
            EventId = ticket.EventId,
            SellerId = ticket.SellerId,
            Name = ticket.Name,
            BasePrice = ticket.BasePrice.Value,
            PremiumPrice = ticket.PremiumPrice?.Value,
            VIPPrice = ticket.VIPPrice?.Value,
            TotalQuantity = ticket.TotalQuantity,
            AvailableQuantity = ticket.AvailableQuantity,
            SalesStartsAt = ticket.SalesStartsAt,
            SalesEndsAt = ticket.SalesEndsAt,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt
        };
    }
}
