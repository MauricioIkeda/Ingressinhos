using System.Text;
using System.Text.Json;
using Generic.Messaging.Interfaces;
using Generic.Messaging.Options;
using RabbitMQ.Client;

namespace Generic.Messaging.Services;

public sealed class RabbitMqMessagePublisher : IMessagePublisher
{
    private readonly RabbitMqOptions _options;
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);

    public RabbitMqMessagePublisher(RabbitMqOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public void Publish<TMessage>(string queueName, TMessage message) where TMessage : class
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        PublishAsync(queueName, message).GetAwaiter().GetResult();
    }

    private async Task PublishAsync<TMessage>(string queueName, TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class
    {
        var normalizedQueueName = RabbitMqTopology.NormalizeQueueName(queueName);
        var factory = RabbitMqTopology.CreateConnectionFactory(_options, "ingressinhos-message-publisher");

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await RabbitMqTopology.DeclareQueueAsync(channel, normalizedQueueName, cancellationToken);

        var envelope = new RabbitMqMessageEnvelope<TMessage>
        {
            MessageId = Guid.NewGuid().ToString("N"),
            MessageType = typeof(TMessage).FullName ?? typeof(TMessage).Name,
            PublishedAtUtc = DateTime.UtcNow,
            Payload = message
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(envelope, _serializerOptions));
        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
            MessageId = envelope.MessageId,
            Type = envelope.MessageType,
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: normalizedQueueName,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }

    private sealed class RabbitMqMessageEnvelope<TMessage>
    {
        public string MessageId { get; init; } = string.Empty;
        public string MessageType { get; init; } = string.Empty;
        public DateTime PublishedAtUtc { get; init; }
        public TMessage? Payload { get; init; }
    }
}
