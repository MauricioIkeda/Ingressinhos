using Payment.Aplication.Transactions.Dtos;
using Payment.Aplication.Transactions.Interfaces;
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

    public static PaymentCheckoutDto ToCheckoutDto(this PaymentTransaction transaction, IMockCheckoutUrlBuilder mockCheckoutUrlBuilder)
    {
        var mockCheckout = MockPaymentCheckoutFactory.Create(transaction, mockCheckoutUrlBuilder);

        return new PaymentCheckoutDto(
            transaction.Id,
            new OrderPaymentSummaryDto(
                transaction.OrderId,
                transaction.Amount.Value,
                transaction.Method,
                transaction.Status),
            new MockQrCodeDto(
                mockCheckout.QrCodePayload,
                mockCheckout.QrCodeImageDataUri,
                mockCheckout.WebhookSimulationUrl));
    }
}
