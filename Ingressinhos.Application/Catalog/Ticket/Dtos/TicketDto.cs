namespace Ingressinhos.Application.Catalog.Dtos;

public class TicketDto
{
    public long TicketId { get; set; }
    public long EventId { get; set; }
    public string Name { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? PremiumPrice { get; set; }
    public decimal? VipPrice { get; set; }
    public int TotalQuantity { get; set; }
    public DateTime SalesStartsAt { get; set; }
    public DateTime SalesEndsAt { get; set; }
    public bool IsActive { get; set; }
}