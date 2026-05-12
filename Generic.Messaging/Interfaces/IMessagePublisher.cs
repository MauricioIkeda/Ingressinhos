namespace Generic.Messaging.Interfaces;

public interface IMessagePublisher
{
    void Publish<TMessage>(string queueName, TMessage message) where TMessage : class;
}
