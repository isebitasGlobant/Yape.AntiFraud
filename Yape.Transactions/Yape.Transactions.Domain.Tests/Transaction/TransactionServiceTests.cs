
using Microsoft.Extensions.Logging;
using Moq;
using Yape.Transactions.Domain.Transaction;
using Yape.Transactions.Domain.Transaction.models;
using Yape.Transactions.Domain.Transaction.portsOut;

namespace Yape.Transactions.Domain.Tests.Transaction
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly Mock<ILogger<TransactionService>> _loggerMock;
        private readonly TransactionService _transactionService;

        public TransactionServiceTests()
        {
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _eventPublisherMock = new Mock<IEventPublisher>();
            _loggerMock = new Mock<ILogger<TransactionService>>();
            _transactionService = new TransactionService(
                _transactionRepositoryMock.Object,
                _eventPublisherMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CreateTransactionAsync_ShouldCreateTransactionSuccessfully()
        {
            // Arrange
            var transaction = new Domain.Transaction.models.Transaction
            {
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid(),
                Value = 100.0m
            };

            _transactionRepositoryMock
                .Setup(repo => repo.SaveAsync(It.IsAny<Domain.Transaction.models.Transaction>()))
                .Returns(Task.CompletedTask);

            _eventPublisherMock
                .Setup(publisher => publisher.PublishAsync(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _transactionService.CreateTransactionAsync(transaction);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TransactionStatus.Pending, result.Status);
            _transactionRepositoryMock.Verify(repo => repo.SaveAsync(It.IsAny<Domain.Transaction.models.Transaction>()), Times.Once);
            _eventPublisherMock.Verify(publisher => publisher.PublishAsync("TransactionCreated", It.IsAny<Domain.Transaction.models.Transaction>()), Times.Once);
        }

        [Fact]
        public async Task GetTransactionByIdAsync_ShouldReturnTransaction_WhenTransactionExists()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var transaction = new Domain.Transaction.models.Transaction { Id = transactionId };

            _transactionRepositoryMock
                .Setup(repo => repo.GetById(transactionId))
                .ReturnsAsync(transaction);

            // Act
            var result = await _transactionService.GetTransactionByIdAsync(transactionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(transactionId, result.Id);
            _transactionRepositoryMock.Verify(repo => repo.GetById(transactionId), Times.Once);
        }

        [Fact]
        public async Task GetTransactionByIdAsync_ShouldReturnNull_WhenTransactionDoesNotExist()
        {
            // Arrange
            var transactionId = Guid.NewGuid();

            _transactionRepositoryMock
                .Setup(repo => repo.GetById(transactionId))
                .ReturnsAsync((Domain.Transaction.models.Transaction)null);

            // Act
            var result = await _transactionService.GetTransactionByIdAsync(transactionId);

            // Assert
            Assert.Null(result);
            _transactionRepositoryMock.Verify(repo => repo.GetById(transactionId), Times.Once);
        }

        [Fact]
        public async Task UpdateTransactionAsync_ShouldUpdateTransactionStatus_WhenTransactionExists()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var storedTransaction = new Domain.Transaction.models.Transaction { Id = transactionId, Status = TransactionStatus.Pending };
            var updatedTransaction = new Domain.Transaction.models.Transaction { Id = transactionId, Status = TransactionStatus.Approved };

            _transactionRepositoryMock
                .Setup(repo => repo.GetById(transactionId))
                .ReturnsAsync(storedTransaction);

            _transactionRepositoryMock
                .Setup(repo => repo.UpdateStatusAsync(It.IsAny<Domain.Transaction.models.Transaction>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _transactionService.UpdateTransactionAsync(updatedTransaction);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TransactionStatus.Approved, result.Status);
            _transactionRepositoryMock.Verify(repo => repo.GetById(transactionId), Times.Once);
            _transactionRepositoryMock.Verify(repo => repo.UpdateStatusAsync(It.IsAny<Domain.Transaction.models.Transaction>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTransactionAsync_ShouldThrowException_WhenTransactionDoesNotExist()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var updatedTransaction = new Domain.Transaction.models.Transaction { Id = transactionId, Status = TransactionStatus.Approved };

            _transactionRepositoryMock
                .Setup(repo => repo.GetById(transactionId))
                .ReturnsAsync((Domain.Transaction.models.Transaction)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _transactionService.UpdateTransactionAsync(updatedTransaction));
            _transactionRepositoryMock.Verify(repo => repo.GetById(transactionId), Times.Once);
            _transactionRepositoryMock.Verify(repo => repo.UpdateStatusAsync(It.IsAny<Domain.Transaction.models.Transaction>()), Times.Never);
        }
    }
}
