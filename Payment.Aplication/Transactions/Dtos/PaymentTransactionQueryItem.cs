using Payment.Domain.Enums;

namespace Payment.Aplication.Transactions.Dtos;

public sealed class PaymentTransactionQueryItem
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
    public string GatewayTransactionId { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? RefusedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
