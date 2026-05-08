namespace Generic.Application.Dtos;

public class PaymentTransactionApiDto
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public int Status { get; set; }
    public string GatewayTransactionId { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? RefusedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}
