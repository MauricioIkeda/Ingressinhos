namespace Generic.Domain.ValueObjects;

public record Price
{
    public decimal Value { get; init; }

    public Price(decimal value)
    {
        if (value < 0)
        {
            throw new Exception("O preco do ingresso nao pode ser negativo");
        }
        
        Value = value;
    }
}