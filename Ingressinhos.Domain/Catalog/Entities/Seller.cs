using Generic.Domain.Entities;

namespace Ingressinhos.Domain.Catalog.Entities;

public class Seller : User
{
    public string TradingName { get; private set; }

    public Seller(string name, string email, string cpf, string tradingName) : base(name, email, cpf)
    {
        if (string.IsNullOrWhiteSpace(tradingName))
        {
            throw new Exception("Deve ser informado o nome comercial do vendedor");
        }

        TradingName = tradingName.Trim();
    }

    public void ChangeTradingName(string tradingName)
    {
        if (string.IsNullOrWhiteSpace(tradingName))
        {
            throw new Exception("Deve ser informado o nome comercial do vendedor");
        }

        TradingName = tradingName.Trim();
    }
}