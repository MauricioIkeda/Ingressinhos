using Ingressinhos.Domain.Catalog.Enums;

namespace Ingressinhos.Application.Catalog.Dtos;

public sealed class EventWithTicketsDto
{
    public long Id { get; set; }
    public long SellerId { get; set; }
    public string SellerTradingName { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public long LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public int? LocationTotalCapacity { get; set; }
    public bool? LocationHasSeats { get; set; }
    public bool HasSeats { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal? BasePrice { get; set; }
    public decimal? PremiumPrice { get; set; }
    public decimal? VIPPrice { get; set; }
    public List<EventTicketWithPricesDto> Tickets { get; set; } = [];
}

public sealed class EventTicketWithPricesDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal? PremiumPrice { get; set; }
    public decimal? VIPPrice { get; set; }
    public int TotalQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public DateTime SalesStartsAt { get; set; }
    public DateTime SalesEndsAt { get; set; }
    public CatalogTicketStatus Status { get; set; }
}
