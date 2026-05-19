namespace Payment.Aplication.Transactions.Interfaces;

public interface IMockCheckoutUrlBuilder
{
    string BuildCheckoutPageUrl(long paymentTransactionId);
    string BuildWebhookSimulationUrl(long paymentTransactionId);
}
