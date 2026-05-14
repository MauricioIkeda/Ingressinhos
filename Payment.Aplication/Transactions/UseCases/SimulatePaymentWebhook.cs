using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Payment.Aplication.Transactions.Dtos;
using Payment.Aplication.Transactions.Interfaces;
using Payment.Domain.Entities;

namespace Payment.Aplication.Transactions.UseCases;

public class SimulatePaymentWebhook : IUseCaseSimulatePaymentWebhook
{
    private readonly IRepositorySession _repositorySession;
    private readonly IUseCaseHandlePaymentNotification _handlePaymentNotification;

    public SimulatePaymentWebhook(IRepositorySession repositorySession, IUseCaseHandlePaymentNotification handlePaymentNotification)
    {
        _repositorySession = repositorySession;
        _handlePaymentNotification = handlePaymentNotification;
    }

    public OperationResult Execute(long paymentTransactionId, SimulatePaymentWebhookDto command)
    {
        if (paymentTransactionId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Pagamento", "Deve ser informado o identificador da transacao."));
        }

        if (command is null || string.IsNullOrWhiteSpace(command.Status))
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Status", "Envie o status que deve ser simulado no webhook."));
        }

        var repositoryQuery = _repositorySession.GetRepositoryQuery();
        var transaction = repositoryQuery.Return<PaymentTransaction>(paymentTransactionId);
        if (transaction is null)
        {
            return OperationResult.NotFound(new MensagemErro("Pagamento", "Transacao de pagamento nao encontrada."));
        }

        return _handlePaymentNotification.Execute(new PaymentNotificationDto
        {
            GatewayTransactionId = transaction.GatewayTransactionId,
            Status = command.Status
        });
    }
}
