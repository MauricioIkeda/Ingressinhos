namespace Generic.Messaging.Interfaces;

public interface IMessageConsumer
{
    int ConsumeAvailable<TMessage>(string queueName, Func<TMessage, bool> handler) where TMessage : class;
}
