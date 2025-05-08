using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Yape.AntiFraud.AdapterOutKafka.Client.Contracts;


namespace Yape.AntiFraud.AdapterOutKafka.Client
{
public class KafkaProducer : IKafkaProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topic;

        public KafkaProducer(IConfiguration configuration, IProducer<string, string> producer)
        {
            var kafkaConfig = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };

            _producer = producer ?? new ProducerBuilder<string, string>(kafkaConfig).Build();

            _topic = configuration["Kafka:Topic"] ?? throw new ArgumentNullException(nameof(configuration), "Kafka topic configuration is missing.");
        }

        public async Task SendMessageAsync(string key, object message)
        {
            try
            {
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                };
                var messageValue = System.Text.Json.JsonSerializer.Serialize(message, options);

                var result = await _producer.ProduceAsync(_topic, new Message<string, string>
                {
                    Key = key,
                    Value = messageValue
                });

                Console.WriteLine($"Message sent to Kafka: {result.TopicPartitionOffset}");
            }
            catch (ProduceException<string, string> ex)
            {
                Console.WriteLine($"Error sending message to Kafka: {ex.Error.Reason}");
                throw;
            }
        }
    }
}
