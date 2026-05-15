using Payment.Domain.Enums;

namespace Payment.Aplication.Refunds.Dtos;

public sealed class RefundQueryItem
{
    public long Id { get; set; }
    public long PaymentTransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public RefundStatus Status { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
