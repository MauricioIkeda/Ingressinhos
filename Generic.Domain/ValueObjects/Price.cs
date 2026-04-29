using Generic.Domain.Entities;

namespace Generic.Domain.ValueObjects;

public class Price : ValidatableObject
{
    public decimal Value { get; init; }

    public Price(decimal value)
    {
        if (value < 0)
        {
            AddError("Price", "O preco do ingresso nao pode ser negativo");
            return;
        }
        
        Value = value;
    }
}
