using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Polly;
using Polly.Retry;
using Serilog;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Yape.AntiFraud.AdapterOutKafka.Client
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
                        Log.Warning("Retry {RetryCount} for HTTP request. Waiting {TimeSpan} before next retry. Response: {Response}",
                            retryCount, timeSpan, result.Result?.StatusCode);
                    });
        }

        public void StartConsuming(string topic, string validateTransactionEndpoint, CancellationToken token = default)
        {
            CreateTopicIfNotExists(topic);

            using var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
            consumer.Subscribe(topic);

            try
            {
                while (!token.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume();
                    Log.Information("Message received: {Message}", consumeResult.Message.Value);

                    var content = new StringContent(consumeResult.Message.Value, Encoding.UTF8, "application/json");

                    // Use the retry policy for the HTTP request
                    var response = _retryPolicy.ExecuteAsync(() =>
                        _httpClient.PostAsync(validateTransactionEndpoint, content)).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        Log.Information("Message processed successfully.");
                    }
                    else
                    {
                        Log.Error("Error processing message after retries: {StatusCode}", response.StatusCode);
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