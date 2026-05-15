using Ingressinhos.Domain.Catalog.Enums;

namespace Ingressinhos.Application.Catalog.Dtos;

public sealed class TicketQueryItem
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public long SellerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal? PremiumPrice { get; set; }
    public decimal? VIPPrice { get; set; }
    public int TotalQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public DateTime SalesStartsAt { get; set; }
    public DateTime SalesEndsAt { get; set; }
    public CatalogTicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
