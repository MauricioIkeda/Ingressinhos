using Payment.Domain.Entities;
using Payment.Domain.Enums;

namespace Payment.Aplication.Transactions.Interfaces;

public interface IPaymentProcessor
{
    RequestPaymentProcessorResult RequestPayment(PaymentTransaction transaction);
    ResolvePaymentProcessorResult ResolvePayment(PaymentTransaction transaction);
    RequestRefundProcessorResult RequestRefund(Refund refund);
    ResolveRefundProcessorResult ResolveRefund(Refund refund);
}

public class RequestPaymentProcessorResult
{
    public string GatewayTransactionId { get; init; } = string.Empty;
}

public class ResolvePaymentProcessorResult
{
    public PaymentStatus Status { get; init; }
}

public class RequestRefundProcessorResult
{
    public string GatewayRefundId { get; init; } = string.Empty;
}

public class ResolveRefundProcessorResult
{
    public RefundStatus Status { get; init; }
}
