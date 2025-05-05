using Confluent.Kafka;
using Microsoft.Extensions.Configuration;


namespace Yape.AntiFraud.AdapterOutKafka
{
public class KafkaProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topic;

        public KafkaProducer(IConfiguration configuration)
        {
            // Configuración del servidor Kafka desde appsettings.json
            var kafkaConfig = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };

            // Inicializa el productor Kafka
            _producer = new ProducerBuilder<string, string>(kafkaConfig).Build();

            // Obtiene el nombre del topic desde la configuración
            _topic = configuration["Kafka:Topic"] ?? throw new ArgumentNullException(nameof(configuration), "Kafka topic configuration is missing.");
        }

        public async Task SendMessageAsync(string key, object message)
        {
            try
            {
                // Serializa el mensaje a JSON
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                };
                var messageValue = System.Text.Json.JsonSerializer.Serialize(message, options);

                // Envía el mensaje al servidor Kafka
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
