using System.Text.Json;
using Generic.Messaging.Interfaces;
using Generic.Messaging.Options;

namespace Generic.Messaging.Services;

public sealed class FileMessageConsumer : IMessageConsumer
{
    private readonly FileMessageBusOptions _options;
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);

    public FileMessageConsumer(FileMessageBusOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public int ConsumeAvailable<TMessage>(string queueName, Func<TMessage, bool> handler) where TMessage : class
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        var pendingDirectory = GetQueueDirectory(queueName, "pending");
        if (!Directory.Exists(pendingDirectory))
        {
            return 0;
        }

        var processingDirectory = GetQueueDirectory(queueName, "processing");
        var processedDirectory = GetQueueDirectory(queueName, "processed");
        var failedDirectory = GetQueueDirectory(queueName, "failed");

        Directory.CreateDirectory(processingDirectory);
        Directory.CreateDirectory(processedDirectory);
        Directory.CreateDirectory(failedDirectory);

        var handledCount = 0;

        foreach (var pendingFile in Directory.EnumerateFiles(pendingDirectory, "*.json").OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
        {
            var processingFile = Path.Combine(processingDirectory, Path.GetFileName(pendingFile));

            try
            {
                // Move para processing antes de ler para reduzir risco de dois consumers pegarem a mesma mensagem.
                File.Move(pendingFile, processingFile);
            }
            catch (IOException)
            {
                continue;
            }

            var processedSuccessfully = false;

            try
            {
                var content = File.ReadAllText(processingFile);
                var envelope = JsonSerializer.Deserialize<FileMessageEnvelope<TMessage>>(content, _serializerOptions);

                if (envelope?.Payload is null)
                {
                    throw new InvalidOperationException("A mensagem encontrada nao possui payload valido.");
                }

                // O handler devolve true quando a mensagem pode ser considerada concluida.
                processedSuccessfully = handler(envelope.Payload);
                if (processedSuccessfully)
                {
                    handledCount++;
                }
            }
            catch
            {
                processedSuccessfully = false;
            }

            // Mensagem bem-sucedida vai para processed; falha vai para failed para inspeção posterior.
            var destinationDirectory = processedSuccessfully ? processedDirectory : failedDirectory;
            var destinationFile = Path.Combine(destinationDirectory, Path.GetFileName(processingFile));

            if (File.Exists(destinationFile))
            {
                File.Delete(destinationFile);
            }

            File.Move(processingFile, destinationFile);
        }

        return handledCount;
    }

    private string GetQueueDirectory(string queueName, string state)
    {
        if (string.IsNullOrWhiteSpace(queueName))
        {
            throw new InvalidOperationException("Deve ser informado o nome da fila.");
        }

        if (string.IsNullOrWhiteSpace(_options.BasePath))
        {
            throw new InvalidOperationException("O caminho base da mensageria nao foi configurado.");
        }

        return Path.Combine(_options.BasePath, queueName.Trim(), state);
    }

    private sealed class FileMessageEnvelope<TMessage>
    {
        public string MessageId { get; init; } = string.Empty;
        public string MessageType { get; init; } = string.Empty;
        public DateTime PublishedAtUtc { get; init; }
        public TMessage? Payload { get; init; }
    }
}
