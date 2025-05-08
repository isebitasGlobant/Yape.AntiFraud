using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Yape.AntiFraud.AdapterOutRepository.postgreSql;
using Yape.AntiFraud.AdapterOutRepository.postgreSql.entities;
using Yape.AntiFraud.AdapterOutRepository.postgreSql.repositories;
using Yape.AntiFraud.Domain.Transaction.models;

namespace Yape.AntiFraud.AdapterOutRepository.Tests.PostgreSql.Repositories
{
    public class TransactionRepositoryTests
    {
        private readonly Mock<PostgreSqlContext> _mockContext;
        private readonly TransactionRepository _repository;

        public TransactionRepositoryTests()
        {
            _mockContext = new Mock<PostgreSqlContext>(new DbContextOptions<PostgreSqlContext>());
            _repository = new TransactionRepository(_mockContext.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnTransaction_WhenTransactionExists()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var transactionEntity = new TransactionEntity
            {
                Id = transactionId,
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid(),
                TransferTypeId = 1,
                Value = 100,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending"
            };

            var dbSetMock = new Mock<DbSet<TransactionEntity>>();
            dbSetMock.Setup(m => m.FindAsync(transactionId)).ReturnsAsync(transactionEntity);
            _mockContext.Setup(c => c.Transactions).Returns(dbSetMock.Object);

            // Act
            var result = await _repository.GetByIdAsync(transactionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(transactionId, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenTransactionDoesNotExist()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var dbSetMock = new Mock<DbSet<TransactionEntity>>();
            dbSetMock.Setup(m => m.FindAsync(transactionId)).ReturnsAsync((TransactionEntity)null);
            _mockContext.Setup(c => c.Transactions).Returns(dbSetMock.Object);

            // Act
            var result = await _repository.GetByIdAsync(transactionId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetTotalAmountForTodaysBySourceAccountIdAsync_ShouldReturnTransactions_WhenTransactionsExist()
        {
            // Arrange
            var sourceAccountId = Guid.NewGuid();
            var today = DateTime.UtcNow.Date;
            var transactions = new List<TransactionEntity>
            {
                new() {
                    Id = Guid.NewGuid(),
                    SourceAccountId = sourceAccountId,
                    TargetAccountId = Guid.NewGuid(),
                    TransferTypeId = 1,
                    Value = 100,
                    CreatedAt = today,
                    Status = "Pending"
                }
            };

            // Use BuildMockDbSet to create a mock DbSet with IQueryable support
            var asyncTransactions = transactions.AsQueryable().BuildMockDbSet();

            _mockContext.Setup(c => c.Transactions).Returns(asyncTransactions.Object);

            // Act
            var result = await _repository.GetTotalAmountForTodaysBySourceAccountIdAsync(sourceAccountId);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(sourceAccountId, result.First().SourceAccountId);
        }

        [Fact]
        public async Task SaveAsync_ShouldAddTransaction()
        {
            // Arrange
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid(),
                TransferTypeId = 1,
                Value = 100,
                CreatedAt = DateTime.UtcNow,
                Status = TransactionStatus.Pending
            };

            var dbSetMock = new Mock<DbSet<TransactionEntity>>();
            _mockContext.Setup(c => c.Transactions).Returns(dbSetMock.Object);

            // Act
            await _repository.SaveAsync(transaction);

            // Assert
            dbSetMock.Verify(m => m.Add(It.IsAny<TransactionEntity>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldUpdateTransactionStatus()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var transactionEntity = new TransactionEntity
            {
                Id = transactionId,
                Status = "Pending"
            };

            var transaction = new Transaction
            {
                Id = transactionId,
                Status = TransactionStatus.Approved
            };

            var dbSetMock = new Mock<DbSet<TransactionEntity>>();
            dbSetMock.Setup(m => m.FindAsync(transactionId)).ReturnsAsync(transactionEntity);
            _mockContext.Setup(c => c.Transactions).Returns(dbSetMock.Object);

            // Act
            await _repository.UpdateStatusAsync(transaction);

            // Assert
            Assert.Equal("Approved", transactionEntity.Status);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }
    }
}
