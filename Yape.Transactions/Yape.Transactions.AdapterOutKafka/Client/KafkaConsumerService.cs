using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Polly;
using Polly.Retry;
using Serilog;
using System.Text;

namespace Yape.Transactions.AdapterOutKafka.Client
{
    public class KafkaConsumerService
    {
        private readonly ConsumerConfig _consumerConfig;
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public KafkaConsumerService(ConsumerConfig consumerConfig, HttpClient httpClient)
        {
            _consumerConfig = consumerConfig;
            _httpClient = httpClient;

            // Define a retry policy with Polly
            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (result, timeSpan, retryCount, context) =>
                    {
                        Log.Warning("Retry {RetryCount} for HTTP request failed with status code {StatusCode}. Retrying in {TimeSpan} seconds.",
                            retryCount, result.Result?.StatusCode, timeSpan.TotalSeconds);
                    });
        }

        public void StartConsuming(string topic, string updateTransactionEndpoint)
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

                    // Send the message to the UpdateTransaction endpoint with retry policy
                    var content = new StringContent(consumeResult.Message.Value, Encoding.UTF8, "application/json");
                    var response = _retryPolicy.ExecuteAsync(() => _httpClient.PostAsync(updateTransactionEndpoint, content)).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        Log.Information("Message processed successfully.");
                    }
                    else
                    {
                        Log.Error("Error processing the message after retries: {StatusCode}", response.StatusCode);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Log.Warning("Consumption canceled.");
            }
            catch (ConsumeException e)
            {
                Log.Error("Error consuming the message: {Reason}", e.Error.Reason);
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error: {Message}", ex.Message);
            }
            finally
            {
                Log.Information("Consumption closed.");
                consumer.Close();
            }
        }

        private void CreateTopicIfNotExists(string topic)
        {
            using var adminClient = new AdminClientBuilder(_consumerConfig).Build();
            try
            {
                // Check if the topic exists, and create it if it does not
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
                Log.Error("Error checking or creating the topic: {Message}", ex.Message);
                return;
            }
        }
    }
}