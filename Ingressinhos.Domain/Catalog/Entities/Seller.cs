using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;

namespace Ingressinhos.Domain.Catalog.Entities;

public class Seller : User
{
    public CNPJ Cnpj { get; private set; } = new(string.Empty);
    public string TradingName { get; private set; } = string.Empty;

    protected Seller()
    {
    }

    public Seller(string name, string email, string cnpj, string tradingName, string userId) : base(name, email, userId)
    {
        var cnpjValue = new CNPJ(cnpj);
        CopyErrorsFrom(cnpjValue);
        if (cnpjValue.IsValid)
        {
            Cnpj = cnpjValue;
        }

        if (string.IsNullOrWhiteSpace(tradingName))
        {
            AddError("TradingName", "Deve ser informado o nome comercial do vendedor");
            return;
        }

        TradingName = tradingName.Trim();
    }

    public void ChangeTradingName(string tradingName)
    {
        ClearErrors();

        if (string.IsNullOrWhiteSpace(tradingName))
        {
            AddError("TradingName", "Deve ser informado o nome comercial do vendedor");
            return;
        }

        TradingName = tradingName.Trim();
    }
}
