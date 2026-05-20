namespace Generic.Messaging.Options;

public sealed class RabbitMqOptions  // configurações que vi que são necessarias
{
    public string HostName { get; init; } = "localhost";
    public int Port { get; init; } = 5672;
    public string UserName { get; init; } = "guest";
    public string Password { get; init; } = "guest";
    public string VirtualHost { get; init; } = "/";
    public bool RequeueOnFailure { get; init; }
    public ushort PrefetchCount { get; init; } = 10;
    public int MaxMessagesPerBatch { get; init; } = 50;
}
