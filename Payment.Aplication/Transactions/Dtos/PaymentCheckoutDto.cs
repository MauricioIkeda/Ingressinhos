using Payment.Domain.Enums;

namespace Payment.Aplication.Transactions.Dtos;

public sealed record PaymentCheckoutDto( long PaymentTransactionId, OrderPaymentSummaryDto Order, MockQrCodeDto QrCode);

public sealed record OrderPaymentSummaryDto( long OrderId, decimal Amount, string Method, PaymentStatus Status);

public sealed record MockQrCodeDto( string Payload, string ImageDataUri, string WebhookSimulationUrl);
