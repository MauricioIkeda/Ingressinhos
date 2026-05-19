using Payment.Aplication.Transactions.Interfaces;

namespace Payment.Api.Extensions;

internal sealed class MockCheckoutUrlBuilder : IMockCheckoutUrlBuilder
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MockCheckoutUrlBuilder(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public string BuildCheckoutPageUrl(long paymentTransactionId)
    {
        return $"{ResolvePublicBaseUrl()}/mock-payment.html?paymentTransactionId={paymentTransactionId}";
    }

    public string BuildWebhookSimulationUrl(long paymentTransactionId)
    {
        return $"{ResolvePublicBaseUrl()}/api/payments/transactions/{paymentTransactionId}/simulate-webhook";
    }

    private string ResolvePublicBaseUrl()
    {
        var configuredBaseUrl = _configuration["MockCheckout:PublicBaseUrl"];
        if (!string.IsNullOrWhiteSpace(configuredBaseUrl))
        {
            return configuredBaseUrl.TrimEnd('/');
        }

        var request = _httpContextAccessor.HttpContext?.Request;
        if (request?.Host.HasValue == true)
        {
            return $"{request.Scheme}://{request.Host.Value}".TrimEnd('/');
        }

        throw new InvalidOperationException("Nao foi possivel determinar a URL publica do mock de pagamento.");
    }
}
