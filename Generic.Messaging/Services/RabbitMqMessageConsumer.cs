using System.Text.Json;
using Generic.Messaging.Interfaces;
using Generic.Messaging.Options;
using RabbitMQ.Client;

namespace Generic.Messaging.Services;

public sealed class RabbitMqMessageConsumer : IMessageConsumer
{
    private readonly RabbitMqOptions _options;
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);

    public RabbitMqMessageConsumer(RabbitMqOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public int ConsumeAvailable<TMessage>(string queueName, Func<TMessage, bool> handler) where TMessage : class
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        return ConsumeAvailableAsync(queueName, handler).GetAwaiter().GetResult();
    }

    private async Task<int> ConsumeAvailableAsync<TMessage>( string queueName, Func<TMessage, bool> handler, CancellationToken cancellationToken = default)
        where TMessage : class
    {
        var normalizedQueueName = RabbitMqTopology.NormalizeQueueName(queueName);
        var factory = RabbitMqTopology.CreateConnectionFactory(_options, "ingressinhos-message-consumer");

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await RabbitMqTopology.DeclareQueueAsync(channel, normalizedQueueName, cancellationToken);
        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: _options.PrefetchCount, global: false, cancellationToken);

        var handledCount = 0;
        var maxMessages = Math.Max(1, _options.MaxMessagesPerBatch);

        for (var i = 0; i < maxMessages; i++)
        {
            var delivery = await channel.BasicGetAsync(normalizedQueueName, autoAck: false, cancellationToken);
            if (delivery is null)
            {
                break;
            }

            var processedSuccessfully = false;

            try
            {
                var envelope = JsonSerializer.Deserialize<RabbitMqMessageEnvelope<TMessage>>(
                    delivery.Body.Span,
                    _serializerOptions);

                if (envelope?.Payload is null)
                {
                    throw new InvalidOperationException("A mensagem encontrada nao possui payload valido.");
                }

                processedSuccessfully = handler(envelope.Payload);
            }
            catch
            {
                processedSuccessfully = false;
            }

            if (processedSuccessfully)
            {
                await channel.BasicAckAsync(delivery.DeliveryTag, multiple: false, cancellationToken);
                handledCount++;
                continue;
            }

            await channel.BasicNackAsync(
                delivery.DeliveryTag,
                multiple: false,
                requeue: _options.RequeueOnFailure,
                cancellationToken);
        }

        return handledCount;
    }

    private sealed class RabbitMqMessageEnvelope<TMessage>
    {
        public string MessageId { get; init; } = string.Empty;
        public string MessageType { get; init; } = string.Empty;
        public DateTime PublishedAtUtc { get; init; }
        public TMessage? Payload { get; init; }
    }
}
