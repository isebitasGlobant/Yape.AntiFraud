using Microsoft.Extensions.Configuration;
using Confluent.Kafka;
using Moq;
using Yape.Transactions.AdapterOutKafka.Client;
namespace Yape.Transactions.AdapterOutKafka.Tests.Client
{
    public class KafkaProducerTests
    {
        private readonly Mock<IProducer<string, string>> _mockProducer;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private KafkaProducer _kafkaProducer;

        public KafkaProducerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration
                .Setup(config => config["Kafka:BootstrapServers"])
                .Returns("localhost:9092");
            _mockConfiguration
                .Setup(config => config["Kafka:Topic"])
                .Returns("test-topic");
            _mockProducer = new Mock<IProducer<string, string>>();
        }

        [Fact]
        public async Task SendMessageAsync_ShouldSendMessageToKafka()
        {
            // Arrange
            var key = "test-key";
            var message = new { Id = 1, Name = "Test" };
            var topic = "test-topic";

            // Mock ProduceAsync to return a completed task
            _mockProducer
                .Setup(p => p.ProduceAsync(
                    topic,
                    It.IsAny<Message<string, string>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeliveryResult<string, string>
                {
                    Topic = topic,
                    Partition = 0,
                    Offset = 0
                });

            _kafkaProducer = new KafkaProducer(_mockConfiguration.Object, _mockProducer.Object);

            // Act
            await _kafkaProducer.SendMessageAsync(key, message);

            // Assert
            _mockProducer.Verify(
                p => p.ProduceAsync(
                    topic,
                    It.Is<Message<string, string>>(m => m.Key == key && m.Value.Contains("Test")),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task SendMessageAsync_ShouldThrowException_WhenKafkaProducerFails()
        {
            // Arrange
            var key = "test-key";
            var message = new { Id = 1, Name = "Test" };
            var topic = "test-topic";

            // Mock ProduceAsync to throw an exception
            _mockProducer
                .Setup(p => p.ProduceAsync(
                    topic,
                    It.IsAny<Message<string, string>>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ProduceException<string, string>(new Error(ErrorCode.Local_MsgTimedOut), null));

            _kafkaProducer = new KafkaProducer(_mockConfiguration.Object, _mockProducer.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ProduceException<string, string>>(() => _kafkaProducer.SendMessageAsync(key, message));
        }

    }
}
