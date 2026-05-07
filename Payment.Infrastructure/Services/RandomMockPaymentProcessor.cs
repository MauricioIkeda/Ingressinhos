using Payment.Aplication.Transactions.Interfaces;
using Payment.Domain.Entities;
using Payment.Domain.Enums;

namespace Payment.Infrastructure.Services;

public class RandomMockPaymentProcessor : IPaymentProcessor
{
    public RequestPaymentProcessorResult RequestPayment(PaymentTransaction transaction)
    {
        return new RequestPaymentProcessorResult
        {
            GatewayTransactionId = $"mock-{Guid.NewGuid():N}"
        };
    }

    public ResolvePaymentProcessorResult ResolvePayment(PaymentTransaction transaction)
    {
        var approved = Random.Shared.Next(0, 100) >= 30;

        return new ResolvePaymentProcessorResult
        {
            Status = approved ? PaymentStatus.Approved : PaymentStatus.Refused
        };
    }

    public RequestRefundProcessorResult RequestRefund(Refund refund)
    {
        return new RequestRefundProcessorResult
        {
            GatewayRefundId = $"mock-refund-{Guid.NewGuid():N}"
        };
    }

    public ResolveRefundProcessorResult ResolveRefund(Refund refund)
    {
        var completed = Random.Shared.Next(0, 100) >= 20;

        return new ResolveRefundProcessorResult
        {
            Status = completed ? RefundStatus.Completed : RefundStatus.Rejected
        };
    }
}
