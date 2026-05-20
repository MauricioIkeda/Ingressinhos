using Generic.Messaging.Contracts;
using Generic.Messaging.Contracts.Payments;
using Generic.Messaging.Interfaces;
using Generic.Worker.Interfaces;
using Ingressinhos.Worker.Consumers.Payment;

namespace Ingressinhos.Worker.Routines;

public class ConsumePaymentMessagesRoutine : IWorkerRoutine // Rotina para consumir as mensagens
{
    private readonly ILogger<ConsumePaymentMessagesRoutine> _logger;
    private readonly IMessageConsumer _messageConsumer;
    private readonly PaymentApprovedConsumer _approvedConsumer;
    private readonly PaymentCancelledConsumer _cancelledConsumer;

    public ConsumePaymentMessagesRoutine(ILogger<ConsumePaymentMessagesRoutine> logger, IMessageConsumer messageConsumer, PaymentApprovedConsumer approvedConsumer, PaymentCancelledConsumer cancelledConsumer)
    {
        _logger = logger;
        _messageConsumer = messageConsumer;
        _approvedConsumer = approvedConsumer;
        _cancelledConsumer = cancelledConsumer;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var approvedMessages = _messageConsumer.ConsumeAvailable<PaymentApprovedIntegrationEvent>(MessageQueues.PaymentApproved, _approvedConsumer.Consume);

        var cancelledMessages = _messageConsumer.ConsumeAvailable<PaymentCancelledIntegrationEvent>(MessageQueues.PaymentCancelled, _cancelledConsumer.Consume);

        if (approvedMessages == 0 && cancelledMessages == 0)
        {
            _logger.LogInformation("Nenhuma mensagem de pagamento disponivel para processar em: {time}", DateTimeOffset.Now);
            return Task.CompletedTask;
        }

        _logger.LogInformation( "Foram processadas {approvedCount} mensagem(ns) PaymentApproved e {cancelledCount} mensagem(ns) PaymentCancelled em: {time}",
            approvedMessages,
            cancelledMessages,
            DateTimeOffset.Now);

        return Task.CompletedTask;
    }
}
