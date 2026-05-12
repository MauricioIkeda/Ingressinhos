using System.Text.Json;
using Generic.Messaging.Interfaces;
using Generic.Messaging.Options;

namespace Generic.Messaging.Services;

public sealed class FileMessagePublisher : IMessagePublisher
{
    private readonly FileMessageBusOptions _options;
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);

    public FileMessagePublisher(FileMessageBusOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public void Publish<TMessage>(string queueName, TMessage message) where TMessage : class
    {
        if (string.IsNullOrWhiteSpace(queueName))
        {
            throw new InvalidOperationException("Deve ser informado o nome da fila.");
        }

        if (string.IsNullOrWhiteSpace(_options.BasePath))
        {
            throw new InvalidOperationException("O caminho base da mensageria nao foi configurado.");
        }

        // Esta implementacao fake usa arquivos para simular uma fila entre processos distintos.
        var pendingDirectory = Path.Combine(_options.BasePath, queueName.Trim(), "pending");
        Directory.CreateDirectory(pendingDirectory);

        var envelope = new FileMessageEnvelope<TMessage>
        {
            MessageId = Guid.NewGuid().ToString("N"),
            MessageType = typeof(TMessage).FullName ?? typeof(TMessage).Name,
            PublishedAtUtc = DateTime.UtcNow,
            Payload = message
        };

        var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}-{envelope.MessageId}.json";
        var tempFile = Path.Combine(pendingDirectory, $"{fileName}.tmp");
        var finalFile = Path.Combine(pendingDirectory, fileName);
        var content = JsonSerializer.Serialize(envelope, _serializerOptions);

        // Grava primeiro em .tmp para evitar que o consumer leia uma mensagem incompleta.
        File.WriteAllText(tempFile, content);
        File.Move(tempFile, finalFile);
    }

    private sealed class FileMessageEnvelope<TMessage>
    {
        public string MessageId { get; init; } = string.Empty;
        public string MessageType { get; init; } = string.Empty;
        public DateTime PublishedAtUtc { get; init; }
        public TMessage? Payload { get; init; }
    }
}
