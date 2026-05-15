namespace Ingressinhos.Application.Catalog.Dtos;

public sealed class SellerQueryItem
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Active { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string TradingName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
