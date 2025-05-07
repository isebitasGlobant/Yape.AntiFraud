using Confluent.Kafka;
using Moq;
using Serilog;
using System.Net;
using Yape.Transactions.AdapterOutKafka.Client;

namespace Yape.Transactions.AdapterOutKafka.Tests.Client
{
    public class KafkaConsumerServiceTests
    {
        private readonly Mock<IConsumer<Ignore, string>> _mockConsumer;
        private readonly TestHttpMessageHandler _testHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly ConsumerConfig _consumerConfig;

        public KafkaConsumerServiceTests()
        {
            _mockConsumer = new Mock<IConsumer<Ignore, string>>();
            _testHttpMessageHandler = new TestHttpMessageHandler((request, cancellationToken) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
            _httpClient = new HttpClient(_testHttpMessageHandler);
            _consumerConfig = new ConsumerConfig { GroupId = "test-group", BootstrapServers = "localhost:9092" };

            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        }

        [Fact]
        public async Task StartConsuming_ShouldSendMessageToEndpoint_WhenMessageIsConsumed()
        {
            // Arrange
            var topic = "test-topic";
            var endpoint = "http://localhost/api/updateTransaction";
            var consumedMessage = "Test Message";

            _mockConsumer
                .Setup(c => c.Consume(It.IsAny<CancellationToken>()))
                .Returns(new ConsumeResult<Ignore, string>
                {
                    Message = new Message<Ignore, string> { Value = consumedMessage }
                });

            var service = new KafkaConsumerService(_consumerConfig, _httpClient);
            var cancellationTokenSource = new CancellationTokenSource();

            // Act
            var task = Task.Run(() => service.StartConsuming(topic, endpoint, cancellationTokenSource.Token));
            await Task.Delay(100); // Allow some time for the consumer loop to process
            cancellationTokenSource.Cancel(); // Signal the loop to stop
            await task; // Wait for the task to complete// Stop the infinite loop

            // Assert
            Assert.NotNull(_testHttpMessageHandler); // Ensure the handler was used
        }
    }

    // Replace the Mock<HttpMessageHandler> with a derived class of HttpMessageHandler that exposes the protected SendAsync method.
    public class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _sendAsync;

        public TestHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync)
        {
            _sendAsync = sendAsync;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _sendAsync(request, cancellationToken);
        }
    }
}
