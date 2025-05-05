using Yape.AntiFraud.AdapterOutKafka;

public class KafkaEventPublisher : IEventPublisher
{
    private readonly KafkaProducer _kafkaProducer;

    public KafkaEventPublisher(KafkaProducer kafkaProducer)
    {
        _kafkaProducer = kafkaProducer;
    }

    public async Task PublishAsync(string topic, object message)
    {
        await _kafkaProducer.SendMessageAsync(topic, message);
    }
}
