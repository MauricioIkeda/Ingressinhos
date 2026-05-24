using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Enums;
using Ingressinhos.Domain.Catalog.Entities;
using CatalogLocation = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.UseCases;

public class EventGetWithTickets
{
    private readonly IReadRepositoryQuery _readRepositoryQuery;

    public EventGetWithTickets(IReadRepositoryQuery readRepositoryQuery)
    {
        _readRepositoryQuery = readRepositoryQuery;
    }

    public OperationResult<List<TOutput>> Execute<TOutput>(Func<IQueryable<EventWithTicketsDto>, IQueryable<TOutput>> query)
    {
        if (query is null)
        {
            return OperationResult<List<TOutput>>.UnprocessableEntity(new MensagemErro("Filtro", "Consulta OData deve ser informada."));
        }

        try
        {
            var events = _readRepositoryQuery.Query<Event>().ToList();
            var eventIds = events.Select(eventEntity => eventEntity.Id).ToArray();
            var locationIds = events.Select(eventEntity => eventEntity.LocationId).Distinct().ToArray();

            var sellerIds = events.Select(eventEntity => eventEntity.SellerId).Distinct().ToArray();

            var locations = _readRepositoryQuery.Query<CatalogLocation>().Where(location => locationIds.Contains(location.Id))
                .ToDictionary(location => location.Id);

            var seller = _readRepositoryQuery.Query<Seller>().Where(seller => sellerIds.Contains(seller.Id))
            .ToDictionary(seller => seller.Id);

            var ticketsByEvent = _readRepositoryQuery.Query<Ticket>().Where(ticket => eventIds.Contains(ticket.EventId)).ToList()
                .GroupBy(ticket => ticket.EventId).ToDictionary(group => group.Key, group => group.Select(ToTicketDto).ToList());

            var result = events.Select(eventEntity => ToEventDto(eventEntity, locations, ticketsByEvent, seller))
                .AsQueryable();

            return OperationResult<List<TOutput>>.Ok(query(result).ToList());
        }
        catch (Exception ex)
        {
            return OperationResult<List<TOutput>>.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }

    private static EventWithTicketsDto ToEventDto( Event eventEntity, IReadOnlyDictionary<long, CatalogLocation> locations,
        IReadOnlyDictionary<long, List<EventTicketWithPricesDto>> ticketsByEvent, IReadOnlyDictionary<long, Seller> seller) //transformando o resultado em dto
    {
        ticketsByEvent.TryGetValue(eventEntity.Id, out var tickets);
        locations.TryGetValue(eventEntity.LocationId, out var location);
        seller.TryGetValue(eventEntity.SellerId, out var sellerEntity);

        tickets ??= [];
        var referenceDate = DateTime.UtcNow;
        var nextActiveTicket = tickets.Where(ticket => ticket.Status == CatalogTicketStatus.Active) // ticket ativo
            .OrderBy(ticket => Math.Abs((ticket.SalesStartsAt - referenceDate).Ticks)) // ordena pelo que tem a venda iniciada mais proxima
            .FirstOrDefault();

        return new EventWithTicketsDto
        {
            Id = eventEntity.Id,
            SellerId = eventEntity.SellerId,
            SellerTradingName = sellerEntity?.TradingName ?? string.Empty,
            Name = eventEntity.Name,
            StartTime = eventEntity.StartTime,
            EndTime = eventEntity.EndTime,
            LocationId = eventEntity.LocationId,
            LocationName = location?.Name ?? string.Empty,
            LocationTotalCapacity = location?.TotalCapacity,
            LocationHasSeats = location?.HasSeats,
            HasSeats = eventEntity.HasSeats,
            Description = eventEntity.Description,
            ImageUrl = eventEntity.ImageUrl,
            BasePrice = nextActiveTicket?.BasePrice,
            PremiumPrice = nextActiveTicket?.PremiumPrice,
            VIPPrice = nextActiveTicket?.VIPPrice,
            Tickets = tickets
        };
    }

    private static EventTicketWithPricesDto ToTicketDto(Ticket ticket)
    {
        return new EventTicketWithPricesDto
        {
            Id = ticket.Id,
            Name = ticket.Name,
            BasePrice = ticket.BasePrice.Value,
            PremiumPrice = ticket.PremiumPrice?.Value,
            VIPPrice = ticket.VIPPrice?.Value,
            TotalQuantity = ticket.TotalQuantity,
            AvailableQuantity = ticket.AvailableQuantity,
            SalesStartsAt = ticket.SalesStartsAt,
            SalesEndsAt = ticket.SalesEndsAt,
            Status = ticket.Status
        };
    }
}
