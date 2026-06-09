using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Generic.Messaging.Contracts;
using Generic.Messaging.Contracts.Payments;
using Generic.Messaging.Interfaces;
using Payment.Aplication.Transactions.Dtos;
using Payment.Aplication.Transactions.Interfaces;
using Payment.Domain.Entities;
using Payment.Domain.Enums;

namespace Payment.Aplication.Transactions.UseCases;

public class HandlePaymentNotification : IUseCaseHandlePaymentNotification
{
    private readonly IRepositorySession _repositorySession;
    private readonly IMessagePublisher _messagePublisher;

    public HandlePaymentNotification(IRepositorySession repositorySession, IMessagePublisher messagePublisher)
    {
        _repositorySession = repositorySession;
        _messagePublisher = messagePublisher;
    }

    public OperationResult Execute(PaymentNotificationDto command)
    {
        if (command is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Pagamento", "Envie os dados da notificacao."));
        }

        if (string.IsNullOrWhiteSpace(command.GatewayTransactionId))
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("GatewayTransactionId", "Deve ser informado o identificador da transacao no gateway."));
        }

        var normalizedStatus = command.Status.Trim().ToLowerInvariant();
        if (normalizedStatus != "approved" && normalizedStatus != "refused" && normalizedStatus != "rejected")
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Status", "O status informado pela notificacao nao e suportado."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var transaction = repositoryQuery.Query<PaymentTransaction>(payment =>
                    payment.GatewayTransactionId == command.GatewayTransactionId.Trim())
                .OrderByDescending(payment => payment.RequestedAt)
                .ThenByDescending(payment => payment.Id)
                .FirstOrDefault();

            if (transaction is null)
            {
                return OperationResult.NotFound(new MensagemErro("Pagamento", "Nao foi encontrada uma transacao para o identificador informado pelo gateway."));
            }

            if (transaction.Status == PaymentStatus.Approved && normalizedStatus == "approved")
            {
                return OperationResult.Ok();
            }

            if (transaction.Status == PaymentStatus.Refused && (normalizedStatus == "refused" || normalizedStatus == "rejected"))
            {
                return OperationResult.Ok();
            }

            if (transaction.Status != PaymentStatus.Requested)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("Status", "A transacao nao pode mais ser alterada pela notificacao."));
            }

            var approvedNow = false;
            var cancelledNow = false;

            if (normalizedStatus == "approved")
            {
                transaction.Approve();
                approvedNow = transaction.IsValid;
            }
            else
            {
                transaction.Refuse();
                cancelledNow = transaction.IsValid;
            }

            if (!transaction.IsValid)
            {
                return OperationResult.UnprocessableEntity(transaction.Errors);
            }

            var repository = _repositorySession.GetRepository();
            repository.Upsert(transaction);
            repository.Flush();

            if (approvedNow)
            {
                _messagePublisher.Publish(
                    MessageQueues.PaymentApproved,
                    new PaymentApprovedIntegrationEvent(
                        transaction.Id,
                        transaction.OrderId,
                        transaction.Amount.Value,
                        transaction.Method,
                        transaction.ApprovedAt ?? DateTime.UtcNow));
            }
            else if (cancelledNow)
            {
                _messagePublisher.Publish(
                    MessageQueues.PaymentCancelled,
                    new PaymentCancelledIntegrationEvent(
                        transaction.Id,
                        transaction.OrderId,
                        transaction.Amount.Value,
                        transaction.Method,
                        normalizedStatus,
                        transaction.RefusedAt ?? DateTime.UtcNow));
            }

            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
