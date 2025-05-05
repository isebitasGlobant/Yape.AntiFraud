using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Serilog;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Yape.AntiFraud.AdapterOutKafka
{
    public class KafkaConsumerService
    {
        private readonly ConsumerConfig _consumerConfig;
        private readonly HttpClient _httpClient;

        public KafkaConsumerService(ConsumerConfig consumerConfig, HttpClient httpClient)
        {
            _consumerConfig = consumerConfig;
            _httpClient = httpClient;
        }

        public void StartConsuming(string topic, string validateTransactionEndpoint)
        {
            CreateTopicIfNotExists(topic);

            using var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
            consumer.Subscribe(topic);

            try
            {
                while (true)
                {
                    var consumeResult = consumer.Consume();
                    Log.Information("Message received: {Message}", consumeResult.Message.Value);

                    var content = new StringContent(consumeResult.Message.Value, Encoding.UTF8, "application/json");
                    var response = _httpClient.PostAsync(validateTransactionEndpoint, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        Log.Information("Message processed successfully.");
                    }
                    else
                    {
                        Log.Error("Error processing message: {StatusCode}", response.StatusCode);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Log.Warning("Listening canceled.");
            }
            catch (ConsumeException e)
            {
                Log.Error("Error consuming message: {Reason}", e.Error.Reason);
            }
            catch (Exception ex)
            {
                Log.Fatal("Unexpected error: {Message}", ex.Message);
            }
            finally
            {
                Log.Information("Consuming closed.");
                consumer.Close();
            }
        }

        private void CreateTopicIfNotExists(string topic)
        {
            using var adminClient = new AdminClientBuilder(_consumerConfig).Build();
            try
            {
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                if (!metadata.Topics.Any(t => t.Topic == topic))
                {
                    adminClient.CreateTopicsAsync(new[]
                    {
                            new TopicSpecification { Name = topic, NumPartitions = 1, ReplicationFactor = 1 }
                        }).Wait();
                    Log.Information("Topic '{Topic}' created.", topic);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error verifying or creating the topic: {Message}", ex.Message);
                return;
            }
        }
    }
}