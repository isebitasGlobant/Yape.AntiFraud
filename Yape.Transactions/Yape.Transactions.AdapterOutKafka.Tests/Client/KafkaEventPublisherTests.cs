using Moq;
using Yape.Transactions.AdapterOutKafka.Client.Contracts;

namespace Yape.Transactions.AdapterOutKafka.Tests.Client
{
    public class KafkaEventPublisherTests
    {
        private readonly Mock<IKafkaProducer> _mockKafkaProducer;
        private readonly KafkaEventPublisher _eventPublisher;

        public KafkaEventPublisherTests()
        {
            _mockKafkaProducer = new Mock<IKafkaProducer>(MockBehavior.Strict); // Mock KafkaProducer
            _eventPublisher = new KafkaEventPublisher(_mockKafkaProducer.Object); // Inject mock and retry policy
        }

        [Fact]
        public async Task PublishAsync_ShouldCallSendMessageAsync_WithCorrectParameters()
        {
            // Arrange
            var topic = "test-topic";
            var message = new { Id = 1, Name = "Test" };

            _mockKafkaProducer
                .Setup(p => p.SendMessageAsync(topic, message))
                .Returns(Task.CompletedTask);

            // Act
            await _eventPublisher.PublishAsync(topic, message);

            // Assert
            _mockKafkaProducer.Verify(
                p => p.SendMessageAsync(topic, message),
                Times.Once);
        }

        [Fact]
        public async Task PublishAsync_ShouldRetry_OnException()
        {
            // Arrange
            var topic = "test-topic";
            var message = new { Id = 1, Name = "Test" };
            var retryCount = 3;
            var callCount = 0;

            _mockKafkaProducer
                .Setup(p => p.SendMessageAsync(topic, message))
                .Callback(() =>
                {
                    callCount++;
                    if (callCount < retryCount)
                    {
                        throw new Exception("Simulated exception");
                    }
                })
                .Returns(Task.CompletedTask);

            // Act
            await _eventPublisher.PublishAsync(topic, message);

            // Assert
            _mockKafkaProducer.Verify(
                p => p.SendMessageAsync(topic, message),
                Times.Exactly(retryCount));
        }

        [Fact]
        public async Task PublishAsync_ShouldThrowException_AfterRetriesExhausted()
        {
            // Arrange
            var topic = "test-topic";
            var message = new { Id = 1, Name = "Test" };

            _mockKafkaProducer
                .Setup(p => p.SendMessageAsync(topic, message))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _eventPublisher.PublishAsync(topic, message));

            _mockKafkaProducer.Verify(
                p => p.SendMessageAsync(topic, message),
                Times.Exactly(4)); // 1 for the original call plus 3 times as per the policy
        }
    }
}
