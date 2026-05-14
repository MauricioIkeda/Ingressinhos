using System.Globalization;
using System.Text;
using Payment.Domain.Entities;
using QRCoder;

namespace Payment.Aplication.Transactions.Utils;

internal static class MockPaymentCheckoutFactory
{
    public static MockPaymentCheckoutInfo Create(PaymentTransaction transaction)
    {
        var payload = string.Create(
            CultureInfo.InvariantCulture,
            $"INGRESSINHOS|MOCK-PIX|ORDER:{transaction.OrderId}|TX:{transaction.GatewayTransactionId}|AMOUNT:{transaction.Amount.Value:0.00}|METHOD:{transaction.Method}");

        return new MockPaymentCheckoutInfo
        {
            QrCodePayload = payload,
            QrCodeImageDataUri = BuildSvgDataUri(payload),
            WebhookSimulationUrl = $"/api/payments/transactions/{transaction.Id}/simulate-webhook"
        };
    }

    private static string BuildSvgDataUri(string payload)
    {
        var svg = BuildSvg(payload);
        var svgBytes = Encoding.UTF8.GetBytes(svg);
        return $"data:image/svg+xml;base64,{Convert.ToBase64String(svgBytes)}";
    }

    private static string BuildSvg(string payload)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        var svgQrCode = new SvgQRCode(qrCodeData);
        return svgQrCode.GetGraphic(20, "#111111", "#ffffff", drawQuietZones: true);
    }

    internal sealed class MockPaymentCheckoutInfo
    {
        public string QrCodePayload { get; init; } = string.Empty;
        public string QrCodeImageDataUri { get; init; } = string.Empty;
        public string WebhookSimulationUrl { get; init; } = string.Empty;
    }
}
