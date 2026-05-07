using Payment.Aplication.Refunds.Dtos;
using Payment.Domain.Entities;

namespace Payment.Aplication.Refunds.Utils;

internal static class RefundMapper
{
    public static RefundDto ToDto(this Refund refund)
    {
        return new RefundDto
        {
            Id = refund.Id,
            PaymentTransactionId = refund.PaymentTransactionId,
            Amount = refund.Amount.Value,
            Reason = refund.Reason,
            Status = refund.Status,
            RequestedAt = refund.RequestedAt,
            CompletedAt = refund.CompletedAt,
            RejectedAt = refund.RejectedAt
        };
    }
}
