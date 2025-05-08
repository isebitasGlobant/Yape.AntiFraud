using Moq;
using Yape.AntiFraud.Domain.Transaction;
using Yape.AntiFraud.Domain.Transaction.portsOut;

namespace Yape.AntiFraud.Domain.Tests.Transaction
{
    public class AntiFraudServiceTests
    {
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly AntiFraudService _antiFraudService;

        public AntiFraudServiceTests()
        {
            _eventPublisherMock = new Mock<IEventPublisher>();
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _antiFraudService = new AntiFraudService(_eventPublisherMock.Object, _transactionRepositoryMock.Object);
        }

        [Fact]
        public async Task ValidateTransaction_ShouldRejectTransaction_WhenValueExceeds2000()
        {
            // Arrange
            var transaction = new Domain.Transaction.models.Transaction
            {
                Id = Guid.NewGuid(),
                Value = 2500,
                Status = Domain.Transaction.models.TransactionStatus.Pending
            };

            _transactionRepositoryMock
                .Setup(repo => repo.GetByIdAsync(transaction.Id))
                .ReturnsAsync(transaction);

            // Act
            var result = await _antiFraudService.ValidateTransaction(transaction);

            // Assert
            Assert.Equal(Domain.Transaction.models.TransactionStatus.Rejected, result.Status);
            _eventPublisherMock.Verify(pub => pub.PublishAsync("TransactionValidated", transaction), Times.Once);
        }

        [Fact]
        public async Task ValidateTransaction_ShouldRejectTransaction_WhenDailyTotalExceeds20000()
        {
            // Arrange
            var transaction = new Domain.Transaction.models.Transaction
            {
                Id = Guid.NewGuid(),
                Value = 1500,
                Status = Domain.Transaction.models.TransactionStatus.Pending,
                SourceAccountId = Guid.NewGuid()
            };

            var dailyTransactions = new List<Domain.Transaction.models.Transaction>
                {
                    new() { Value = 10000, Status = Domain.Transaction.models.TransactionStatus.Approved },
                    new() { Value = 11000, Status = Domain.Transaction.models.TransactionStatus.Approved }
                };

            _transactionRepositoryMock
                .Setup(repo => repo.GetByIdAsync(transaction.Id))
                .ReturnsAsync(transaction);

            _transactionRepositoryMock
                .Setup(repo => repo.GetTotalAmountForTodaysBySourceAccountIdAsync(transaction.SourceAccountId))
                .ReturnsAsync(dailyTransactions);

            // Act
            var result = await _antiFraudService.ValidateTransaction(transaction);

            // Assert
            Assert.Equal(Domain.Transaction.models.TransactionStatus.Rejected, result.Status);
            _eventPublisherMock.Verify(pub => pub.PublishAsync("TransactionValidated", transaction), Times.Once);
        }

        [Fact]
        public async Task ValidateTransaction_ShouldApproveTransaction_WhenDailyTotalIsWithinLimit()
        {
            // Arrange
            var transaction = new Domain.Transaction.models.Transaction
            {
                Id = Guid.NewGuid(),
                Value = 1500,
                Status = Domain.Transaction.models.TransactionStatus.Pending,
                SourceAccountId = Guid.NewGuid()
            };

            var dailyTransactions = new List<Domain.Transaction.models.Transaction>
                {
                    new() { Value = 5000, Status = Domain.Transaction.models.TransactionStatus.Approved },
                    new() { Value = 10000, Status = Domain.Transaction.models.TransactionStatus.Approved }
                };

            _transactionRepositoryMock
                .Setup(repo => repo.GetByIdAsync(transaction.Id))
                .ReturnsAsync(transaction);

            _transactionRepositoryMock
                .Setup(repo => repo.GetTotalAmountForTodaysBySourceAccountIdAsync(transaction.SourceAccountId))
                .ReturnsAsync(dailyTransactions);

            // Act
            var result = await _antiFraudService.ValidateTransaction(transaction);

            // Assert
            Assert.Equal(Domain.Transaction.models.TransactionStatus.Approved, result.Status);
            _eventPublisherMock.Verify(pub => pub.PublishAsync("TransactionValidated", transaction), Times.Once);
        }

        [Fact]
        public async Task ValidateTransaction_ShouldApproveTransaction_WhenStatusIsNotPending()
        {
            // Arrange
            var transaction = new Domain.Transaction.models.Transaction
            {
                Id = Guid.NewGuid(),
                Value = 1500,
                Status = Domain.Transaction.models.TransactionStatus.Approved
            };

            _transactionRepositoryMock
                .Setup(repo => repo.GetByIdAsync(transaction.Id))
                .ReturnsAsync(transaction);

            // Act
            var result = await _antiFraudService.ValidateTransaction(transaction);

            // Assert
            Assert.Equal(Domain.Transaction.models.TransactionStatus.Approved, result.Status);
            _eventPublisherMock.Verify(pub => pub.PublishAsync("TransactionValidated", transaction), Times.Once);
        }
    }
}
