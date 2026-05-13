namespace Ingressinhos.Application.Catalog.Dtos;

public class SellerDto
{
    public long SellerId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Cnpj { get; set; }
    public string Password { get; set; } // somente para inclusão
    public string TradingName { get; set; }
}

public record SellerGet( long SellerId, string Name, string Email, string Cnpj, string TradingName );