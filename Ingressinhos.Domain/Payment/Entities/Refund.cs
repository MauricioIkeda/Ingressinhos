using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;
using Ingressinhos.Domain.Payment.Enums;

namespace Ingressinhos.Domain.Payment.Entities;

public class Refund : BaseEntity
{
    public long PaymentTransactionId { get; private set; }
    public Price Amount { get; private set; } = new(0);
    public string Reason { get; private set; } = string.Empty;
    public RefundStatus Status { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? RejectedAt { get; private set; }

    protected Refund()
    {
    }

    public Refund(long paymentTransactionId, decimal amount, string reason)
    {
        if (paymentTransactionId <= 0)
        {
            AddError("PaymentTransactionId", "Deve ser informado o pagamento do reembolso");
        }
        else
        {
            PaymentTransactionId = paymentTransactionId;
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            AddError("Reason", "Deve ser informada a justificativa do reembolso");
        }
        else
        {
            Reason = reason.Trim();
        }

        var price = new Price(amount);
        CopyErrorsFrom(price);
        if (price.IsValid)
        {
            Amount = price;
        }

        Status = RefundStatus.Requested;
        RequestedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        ClearErrors();

        if (Status != RefundStatus.Requested)
        {
            AddError("Status", "Somente reembolsos solicitados podem ser finalizados");
            return;
        }

        Status = RefundStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        ClearErrors();

        if (Status != RefundStatus.Requested)
        {
            AddError("Status", "Somente reembolsos solicitados podem ser rejeitados");
            return;
        }

        Status = RefundStatus.Rejected;
        RejectedAt = DateTime.UtcNow;
    }
}
