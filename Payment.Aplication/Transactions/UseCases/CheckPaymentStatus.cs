using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Generic.Messaging.Contracts;
using Generic.Messaging.Interfaces;
using Payment.Aplication.Transactions.Dtos;
using Payment.Aplication.Transactions.Interfaces;
using Payment.Aplication.Transactions.Utils;
using Payment.Domain.Entities;
using Payment.Domain.Enums;

namespace Payment.Aplication.Transactions.UseCases;

public class CheckPaymentStatus : IUseCaseCheckPaymentStatus
{
    private readonly IRepositorySession _repositorySession;
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IMessagePublisher _messagePublisher;

    public CheckPaymentStatus(IRepositorySession repositorySession, IPaymentProcessor paymentProcessor, IMessagePublisher messagePublisher)
    {
        _repositorySession = repositorySession;
        _paymentProcessor = paymentProcessor;
        _messagePublisher = messagePublisher;
    }

    public OperationResult<PaymentTransactionDto> Execute(long orderId)
    {
        if (orderId <= 0)
        {
            return OperationResult<PaymentTransactionDto>.UnprocessableEntity(new MensagemErro("Pedido", "Deve ser informado o identificador do pedido."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            // O status e resolvido sempre sobre a transacao mais recente do pedido.
            var transaction = repositoryQuery.Query<PaymentTransaction>(payment => payment.OrderId == orderId)
                .OrderByDescending(payment => payment.RequestedAt)
                .ThenByDescending(payment => payment.Id)
                .FirstOrDefault();

            if (transaction is null)
            {
                return OperationResult<PaymentTransactionDto>.NotFound(new MensagemErro("Pagamento", "Nenhuma transacao de pagamento foi encontrada para o pedido informado."));
            }

            if (transaction.Status == PaymentStatus.Requested)
            {
                // Aqui esta o ponto de integracao com o gateway/processador de pagamento.
                var processorResult = _paymentProcessor.ResolvePayment(transaction);
                var approvedNow = false;

                if (processorResult.Status == PaymentStatus.Approved)
                {
                    transaction.Approve();
                    approvedNow = transaction.IsValid;
                }
                else if (processorResult.Status == PaymentStatus.Refused)
                {
                    transaction.Refuse();
                }

                if (!transaction.IsValid)
                {
                    return OperationResult<PaymentTransactionDto>.UnprocessableEntity(transaction.Errors);
                }

                var repository = _repositorySession.GetRepository();
                repository.Upsert(transaction);
                repository.Flush().GetAwaiter().GetResult();

                if (approvedNow)
                {
                    // O contexto Payment nao atualiza Order diretamente; ele publica um fato.
                    _messagePublisher.Publish(
                        MessageQueues.PaymentApproved,
                        new PaymentApprovedIntegrationEvent
                        {
                            PaymentTransactionId = transaction.Id,
                            OrderId = transaction.OrderId,
                            Amount = transaction.Amount.Value,
                            Method = transaction.Method,
                            ApprovedAtUtc = transaction.ApprovedAt ?? DateTime.UtcNow
                        });
                }
            }

            return OperationResult<PaymentTransactionDto>.Ok(transaction.ToDto());
        }
        catch (Exception ex)
        {
            return OperationResult<PaymentTransactionDto>.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
