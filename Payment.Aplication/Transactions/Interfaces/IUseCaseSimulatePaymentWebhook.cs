using Generic.Domain.Entities;
using Payment.Aplication.Transactions.Dtos;

namespace Payment.Aplication.Transactions.Interfaces;

public interface IUseCaseSimulatePaymentWebhook
{
    OperationResult Execute(long paymentTransactionId, SimulatePaymentWebhookDto command);
}
