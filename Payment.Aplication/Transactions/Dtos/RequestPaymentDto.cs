namespace Payment.Aplication.Transactions.Dtos;

public class RequestPaymentDto
{
    public long OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
}
