using Microsoft.EntityFrameworkCore;
using Moq;
using Yape.Transactions.AdapterOutRepository.postgreSql;
using Yape.Transactions.AdapterOutRepository.postgreSql.entities;
using Yape.Transactions.AdapterOutRepository.postgreSql.Repositories;
using Yape.Transactions.Domain.Transaction.models;

namespace Yape.Transactions.AdapterOutRepository.Tests.PostgreSql.Repositories
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
        public async Task GetById_ShouldReturnTransaction_WhenTransactionExists()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var transactionEntity = new TransactionEntity
            {
                Id = transactionId,
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid(),
                TransferTypeId = 1,
                Value = 100.00m,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending"
            };

            var dbSetMock = new Mock<DbSet<TransactionEntity>>();
            dbSetMock.Setup(m => m.FindAsync(transactionId)).ReturnsAsync(transactionEntity);
            _mockContext.Setup(c => c.Transactions).Returns(dbSetMock.Object);

            // Act
            var result = await _repository.GetById(transactionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(transactionId, result.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenTransactionDoesNotExist()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var dbSetMock = new Mock<DbSet<TransactionEntity>>();
            dbSetMock.Setup(m => m.FindAsync(transactionId)).ReturnsAsync((TransactionEntity)null);
            _mockContext.Setup(c => c.Transactions).Returns(dbSetMock.Object);

            // Act
            var result = await _repository.GetById(transactionId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SaveAsync_ShouldAddTransactionAndSaveChanges()
        {
            // Arrange
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid(),
                TransferTypeId = 1,
                Value = 100.00m,
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
        public async Task UpdateStatusAsync_ShouldUpdateTransactionStatus_WhenTransactionExists()
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

        [Fact]
        public async Task UpdateStatusAsync_ShouldDoNothing_WhenTransactionDoesNotExist()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var transaction = new Transaction
            {
                Id = transactionId,
                Status = TransactionStatus.Approved
            };

            var dbSetMock = new Mock<DbSet<TransactionEntity>>();
            dbSetMock.Setup(m => m.FindAsync(transactionId)).ReturnsAsync((TransactionEntity)null);
            _mockContext.Setup(c => c.Transactions).Returns(dbSetMock.Object);

            // Act
            await _repository.UpdateStatusAsync(transaction);

            // Assert
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Never);
        }
    }
}
