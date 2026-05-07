namespace Payment.Aplication.Refunds.Dtos;

public class RequestRefundDto
{
    public long PaymentTransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
}
