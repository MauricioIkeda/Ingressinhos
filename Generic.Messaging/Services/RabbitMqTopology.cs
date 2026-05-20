using RabbitMQ.Client;

namespace Generic.Messaging.Services;

internal static class RabbitMqTopology // Responsável por definir a topologia de filas e exchanges no RabbitMQ, bem como fornecer métodos auxiliares para criação de conexão
{
    public const string FailedQueueSuffix = ".failed";

    public static ConnectionFactory CreateConnectionFactory(
        Generic.Messaging.Options.RabbitMqOptions options,
        string clientProvidedName)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return new ConnectionFactory
        {
            HostName = string.IsNullOrWhiteSpace(options.HostName) ? "localhost" : options.HostName.Trim(),
            Port = options.Port <= 0 ? 5672 : options.Port,
            UserName = string.IsNullOrWhiteSpace(options.UserName) ? "guest" : options.UserName,
            Password = string.IsNullOrWhiteSpace(options.Password) ? "guest" : options.Password,
            VirtualHost = string.IsNullOrWhiteSpace(options.VirtualHost) ? "/" : options.VirtualHost,
            ClientProvidedName = clientProvidedName,
            AutomaticRecoveryEnabled = true
        };
    }

    public static async Task DeclareQueueAsync(IChannel channel, string queueName, CancellationToken cancellationToken = default)
    {
        var normalizedQueueName = NormalizeQueueName(queueName);
        var failedQueueName = GetFailedQueueName(normalizedQueueName);

        await channel.QueueDeclareAsync(
            queue: failedQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        var arguments = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = string.Empty,
            ["x-dead-letter-routing-key"] = failedQueueName
        };

        await channel.QueueDeclareAsync(
            queue: normalizedQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: arguments,
            cancellationToken: cancellationToken);
    }

    public static string NormalizeQueueName(string queueName)
    {
        if (string.IsNullOrWhiteSpace(queueName))
        {
            throw new InvalidOperationException("Deve ser informado o nome da fila.");
        }

        return queueName.Trim();
    }

    private static string GetFailedQueueName(string queueName)
    {
        return $"{queueName}{FailedQueueSuffix}";
    }
}
