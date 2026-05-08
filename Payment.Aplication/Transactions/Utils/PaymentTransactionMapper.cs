using Payment.Aplication.Transactions.Dtos;
using Payment.Domain.Entities;

namespace Payment.Aplication.Transactions.Utils;

internal static class PaymentTransactionMapper
{
    public static PaymentTransactionDto ToDto(this PaymentTransaction transaction)
    {
        return new PaymentTransactionDto
        {
            Id = transaction.Id,
            OrderId = transaction.OrderId,
            Amount = transaction.Amount.Value,
            Method = transaction.Method,
            Status = transaction.Status,
            GatewayTransactionId = transaction.GatewayTransactionId,
            RequestedAt = transaction.RequestedAt,
            ApprovedAt = transaction.ApprovedAt,
            RefusedAt = transaction.RefusedAt,
            CancelledAt = transaction.CancelledAt
        };
    }
}
