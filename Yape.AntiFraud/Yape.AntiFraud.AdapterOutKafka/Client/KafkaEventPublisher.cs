using Polly;
using Polly.Retry;
using Yape.AntiFraud.AdapterOutKafka.Client;

public class KafkaEventPublisher : IEventPublisher
{
    private readonly KafkaProducer _kafkaProducer;
    private readonly AsyncRetryPolicy _retryPolicy;

    public KafkaEventPublisher(KafkaProducer kafkaProducer)
    {
        _kafkaProducer = kafkaProducer;

        // Define a retry policy with Polly
        _retryPolicy = Policy
            .Handle<Exception>() // Retry on any exception
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))); // Exponential backoff
    }

    public async Task PublishAsync(string topic, object message)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            await _kafkaProducer.SendMessageAsync(topic, message);
        });
    }
}
