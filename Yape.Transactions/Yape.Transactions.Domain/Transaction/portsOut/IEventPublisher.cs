public interface IEventPublisher
{
    Task PublishAsync(string topic, object message);
}
