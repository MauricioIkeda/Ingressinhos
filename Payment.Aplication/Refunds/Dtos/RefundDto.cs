using Payment.Domain.Enums;

namespace Payment.Aplication.Refunds.Dtos;

public class RefundDto
{
    public long Id { get; set; }
    public long PaymentTransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public RefundStatus Status { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
}
