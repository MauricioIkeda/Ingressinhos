using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;
using Ingressinhos.Domain.Payment.Enums;

namespace Ingressinhos.Domain.Payment.Entities;

public class Refund : BaseEntity
{
    public long PaymentTransactionId { get; private set; }
    public Price Amount { get; private set; }
    public string Reason { get; private set; }
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
            throw new Exception("Deve ser informado o pagamento do reembolso");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new Exception("Deve ser informada a justificativa do reembolso");
        }

        PaymentTransactionId = paymentTransactionId;
        Amount = new Price(amount);
        Reason = reason.Trim();
        Status = RefundStatus.Requested;
        RequestedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != RefundStatus.Requested)
        {
            throw new Exception("Somente reembolsos solicitados podem ser finalizados");
        }

        Status = RefundStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        if (Status != RefundStatus.Requested)
        {
            throw new Exception("Somente reembolsos solicitados podem ser rejeitados");
        }

        Status = RefundStatus.Rejected;
        RejectedAt = DateTime.UtcNow;
    }
}