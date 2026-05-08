namespace Generic.Application.Dtos;

public class RequestPaymentApiDto
{
    public long OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
}
