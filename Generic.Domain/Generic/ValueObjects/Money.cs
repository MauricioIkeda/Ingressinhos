namespace Generic.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; }

    public Money(decimal amount, string currency = "BRL")
    {
        if (amount < 0)
        {
            throw new Exception("O valor monetario nao pode ser negativo");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new Exception("Deve ser informada a moeda");
        }

        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Currency = currency.Trim().ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);

        if (other.Amount > Amount)
        {
            throw new Exception("Nao eh possivel subtrair valor maior do que o atual");
        }

        return new Money(Amount - other.Amount, Currency);
    }

    private void EnsureSameCurrency(Money other)
    {
        if (other is null)
        {
            throw new Exception("Deve ser informado um valor monetario");
        }

        if (Currency != other.Currency)
        {
            throw new Exception("Nao eh possivel operar valores de moedas diferentes");
        }
    }

    public override string ToString()
    {
        return $"{Amount:0.00} {Currency}";
    }
}