using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;

namespace Ingressinhos.Domain.Catalog.Entities;

public class Seller : User
{
    public CNPJ Cnpj { get; private set; }
    public string TradingName { get; private set; }

    protected Seller()
    {
        
    }

    public Seller(string name, string email, string cnpj, string tradingName, string userId) : base(name, email, userId)
    {
        Cnpj = new CNPJ(cnpj);

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