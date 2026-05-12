namespace Payment.Worker.Options;

public sealed class PaymentExpirationOptions
{
    public int CancelAfterMinutes { get; init; } = 30;
}
