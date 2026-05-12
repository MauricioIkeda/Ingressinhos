using Generic.Messaging.Contracts;
using Generic.Messaging.Interfaces;
using Generic.Worker.Interfaces;
using Ingressinhos.Worker.Consumers;

namespace Ingressinhos.Worker.Routines;

public class ConsumePaymentApprovedMessagesRoutine : IWorkerRoutine
{
    private readonly ILogger<ConsumePaymentApprovedMessagesRoutine> _logger;
    private readonly IMessageConsumer _messageConsumer;
    private readonly PaymentApprovedConsumer _consumer;

    public ConsumePaymentApprovedMessagesRoutine(
        ILogger<ConsumePaymentApprovedMessagesRoutine> logger,
        IMessageConsumer messageConsumer,
        PaymentApprovedConsumer consumer)
    {
        _logger = logger;
        _messageConsumer = messageConsumer;
        _consumer = consumer;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // A rotina nao conhece regra de negocio; ela apenas entrega a mensagem ao consumer certo.
        var processedMessages = _messageConsumer.ConsumeAvailable<PaymentApprovedIntegrationEvent>(
            MessageQueues.PaymentApproved,
            _consumer.Consume);

        if (processedMessages == 0)
        {
            _logger.LogInformation("Nenhuma mensagem PaymentApproved disponivel para processar em: {time}", DateTimeOffset.Now);
            return Task.CompletedTask;
        }

        _logger.LogInformation("Foram processadas {count} mensagem(ns) PaymentApproved em: {time}", processedMessages, DateTimeOffset.Now);
        return Task.CompletedTask;
    }
}
