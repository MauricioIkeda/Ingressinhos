using Generic.Domain.Entities;

namespace Generic.Domain.ValueObjects;

public class Money : ValidatableObject
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "BRL";

    public Money(decimal amount, string currency = "BRL")
    {
        if (amount < 0)
        {
            AddError("Amount", "O valor monetario nao pode ser negativo");
            return;
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            AddError("Currency", "Deve ser informada a moeda");
            return;
        }

        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Currency = currency.Trim().ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        ClearErrors();

        if (!EnsureSameCurrency(other))
        {
            return this;
        }

        var result = new Money(Amount + other.Amount, Currency);
        CopyErrorsFrom(result);
        return result;
    }

    public Money Subtract(Money other)
    {
        ClearErrors();

        if (!EnsureSameCurrency(other))
        {
            return this;
        }

        if (other.Amount > Amount)
        {
            AddError("Amount", "Nao eh possivel subtrair valor maior do que o atual");
            return this;
        }

        var result = new Money(Amount - other.Amount, Currency);
        CopyErrorsFrom(result);
        return result;
    }

    private bool EnsureSameCurrency(Money other)
    {
        if (other is null)
        {
            AddError("Money", "Deve ser informado um valor monetario");
            return false;
        }

        if (!other.IsValid)
        {
            CopyErrorsFrom(other);
            return false;
        }

        if (Currency != other.Currency)
        {
            AddError("Currency", "Nao eh possivel operar valores de moedas diferentes");
            return false;
        }

        return true;
    }

    public override string ToString()
    {
        return $"{Amount:0.00} {Currency}";
    }
}
