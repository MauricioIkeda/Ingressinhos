using Generic.Application.Crud.UseCases;
using Generic.Infrastructure.Interfaces;
using Payment.Aplication.Transactions.Dtos;
using Payment.Domain.Entities;

namespace Payment.Aplication.Transactions.UseCases;

public class PaymentTransactionGetOdata : UseCaseGetQueryItems<PaymentTransaction, PaymentTransactionQueryItem>
{
    public PaymentTransactionGetOdata(IRepositoryQuery repositoryQuery)
        : base(repositoryQuery)
    {
    }

    protected override PaymentTransactionQueryItem ToQueryItem(PaymentTransaction paymentTransaction)
    {
        return new PaymentTransactionQueryItem
        {
            Id = paymentTransaction.Id,
            OrderId = paymentTransaction.OrderId,
            Amount = paymentTransaction.Amount.Value,
            Method = paymentTransaction.Method,
            Status = paymentTransaction.Status,
            GatewayTransactionId = paymentTransaction.GatewayTransactionId,
            RequestedAt = paymentTransaction.RequestedAt,
            ApprovedAt = paymentTransaction.ApprovedAt,
            RefusedAt = paymentTransaction.RefusedAt,
            CancelledAt = paymentTransaction.CancelledAt,
            CreatedAt = paymentTransaction.CreatedAt,
            UpdatedAt = paymentTransaction.UpdatedAt
        };
    }
}
