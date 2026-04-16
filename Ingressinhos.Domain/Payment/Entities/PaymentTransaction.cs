using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;
using Ingressinhos.Domain.Payment.Enums;
using Ingressinhos.Domain.Sales.Entities;

namespace Ingressinhos.Domain.Payment.Entities;

public class PaymentTransaction : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Price Amount { get; private set; }
    public string Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string GatewayTransactionId { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime? RefusedAt { get; private set; }

    protected PaymentTransaction()
    {
        
    }

    public PaymentTransaction(Guid orderId, decimal amount, string method)
    {
        if (orderId == Guid.Empty)
        {
            throw new Exception("Deve ser informado o pedido do pagamento");
        }

        if (string.IsNullOrWhiteSpace(method))
        {
            throw new Exception("Deve ser informado o metodo de pagamento");
        }

        Id = Guid.NewGuid();
        OrderId = orderId;
        Amount = new Price(amount);
        Method = method.Trim();
        Status = PaymentStatus.Requested;
        RequestedAt = DateTime.UtcNow;
    }

    public void AttachGatewayId(string gatewayTransactionId)
    {
        if (string.IsNullOrWhiteSpace(gatewayTransactionId))
        {
            throw new Exception("Deve ser informado o id da transacao no gateway");
        }

        GatewayTransactionId = gatewayTransactionId.Trim();
    }

    public void Approve()
    {
        if (Status != PaymentStatus.Requested)
        {
            throw new Exception("Somente pagamentos solicitados podem ser aprovados");
        }

        Status = PaymentStatus.Approved;
        ApprovedAt = DateTime.UtcNow;
    }

    public void Refuse()
    {
        if (Status != PaymentStatus.Requested)
        {
            throw new Exception("Somente pagamentos solicitados podem ser recusados");
        }

        Status = PaymentStatus.Refused;
        RefusedAt = DateTime.UtcNow;
    }
}