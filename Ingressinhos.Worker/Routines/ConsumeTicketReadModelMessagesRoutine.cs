using Generic.Messaging.Contracts;
using Generic.Messaging.Contracts.Tickets;
using Generic.Messaging.Interfaces;
using Generic.Worker.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.Interfaces;
using Ingressinhos.Worker.Consumers.TicketReadModel;

namespace Ingressinhos.Worker.Routines;

public class ConsumeTicketReadModelMessagesRoutine : IWorkerRoutine
{
    private readonly ILogger<ConsumeTicketReadModelMessagesRoutine> _logger;
    private readonly IMessageConsumer _messageConsumer;
    private readonly IClientTicketReadModelHealthCheck _healthCheck;
    private readonly TicketReadModelSyncConsumer _consumer;

    public ConsumeTicketReadModelMessagesRoutine(
        ILogger<ConsumeTicketReadModelMessagesRoutine> logger,
        IMessageConsumer messageConsumer,
        IClientTicketReadModelHealthCheck healthCheck,
        TicketReadModelSyncConsumer consumer)
    {
        _logger = logger;
        _messageConsumer = messageConsumer;
        _healthCheck = healthCheck;
        _consumer = consumer;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (!_healthCheck.IsAvailable())
        {
            _logger.LogWarning("MongoDB indisponivel. As mensagens de read model de tickets permanecerao na fila.");
            return Task.CompletedTask;
        }

        var handledMessages = _messageConsumer.ConsumeAvailable<TicketReadModelSyncIntegrationEvent>(
            MessageQueues.TicketReadModelSync,
            _consumer.Consume);

        if (handledMessages == 0)
        {
            _logger.LogInformation("Nenhuma mensagem de read model de tickets disponivel em: {time}", DateTimeOffset.Now);
            return Task.CompletedTask;
        }

        _logger.LogInformation(
            "Foram processadas {count} mensagem(ns) de read model de tickets em: {time}",
            handledMessages,
            DateTimeOffset.Now);

        return Task.CompletedTask;
    }
}
