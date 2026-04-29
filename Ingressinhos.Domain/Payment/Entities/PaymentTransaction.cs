using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;
using Ingressinhos.Domain.Payment.Enums;

namespace Ingressinhos.Domain.Payment.Entities;

public class PaymentTransaction : BaseEntity
{
    public long OrderId { get; private set; }
    public Price Amount { get; private set; } = new(0);
    public string Method { get; private set; } = string.Empty;
    public PaymentStatus Status { get; private set; }
    public string GatewayTransactionId { get; private set; } = string.Empty;
    public DateTime RequestedAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime? RefusedAt { get; private set; }

    protected PaymentTransaction()
    {
    }

    public PaymentTransaction(long orderId, decimal amount, string method)
    {
        if (orderId <= 0)
        {
            AddError("OrderId", "Deve ser informado o pedido do pagamento");
        }
        else
        {
            OrderId = orderId;
        }

        if (string.IsNullOrWhiteSpace(method))
        {
            AddError("Method", "Deve ser informado o metodo de pagamento");
        }
        else
        {
            Method = method.Trim();
        }

        var price = new Price(amount);
        CopyErrorsFrom(price);
        if (price.IsValid)
        {
            Amount = price;
        }

        Status = PaymentStatus.Requested;
        RequestedAt = DateTime.UtcNow;
    }

    public void AttachGatewayId(string gatewayTransactionId)
    {
        ClearErrors();

        if (string.IsNullOrWhiteSpace(gatewayTransactionId))
        {
            AddError("GatewayTransactionId", "Deve ser informado o id da transacao no gateway");
            return;
        }

        GatewayTransactionId = gatewayTransactionId.Trim();
    }

    public void Approve()
    {
        ClearErrors();

        if (Status != PaymentStatus.Requested)
        {
            AddError("Status", "Somente pagamentos solicitados podem ser aprovados");
            return;
        }

        Status = PaymentStatus.Approved;
        ApprovedAt = DateTime.UtcNow;
    }

    public void Refuse()
    {
        ClearErrors();

        if (Status != PaymentStatus.Requested)
        {
            AddError("Status", "Somente pagamentos solicitados podem ser recusados");
            return;
        }

        Status = PaymentStatus.Refused;
        RefusedAt = DateTime.UtcNow;
    }
}
