namespace Generic.Application.Dtos;

public sealed record PaymentCheckoutApiDto(
    long PaymentTransactionId,
    OrderPaymentSummaryApiDto Order,
    MockQrCodeApiDto QrCode);

public sealed record OrderPaymentSummaryApiDto(
    long OrderId,
    decimal Amount,
    string Method,
    int Status);

public sealed record MockQrCodeApiDto(
    string Payload,
    string ImageDataUri,
    string WebhookSimulationUrl);
