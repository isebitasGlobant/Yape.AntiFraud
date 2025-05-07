using Yape.Transactions.AdapterOutRepository.postgreSql.entities;
using Yape.Transactions.Domain.Transaction.models;
using Yape.Transactions.AdapterOutRepository.postgreSql.Mapper;

namespace Yape.Transactions.AdapterOutRepository.Tests.PostgreSql.Mapper
{
    public class TransactionMapperTests
    {
        [Fact]
        public void ToDomain_ShouldReturnNull_WhenEntityIsNull()
        {
            // Arrange
            TransactionEntity? entity = null;

            // Act
            var result = entity.ToDomain();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ToDomain_ShouldMapEntityToDomainCorrectly()
        {
            // Arrange
            var entity = new TransactionEntity
            {
                Id = Guid.NewGuid(),
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid(),
                TransferTypeId = 1,
                Value = 100.50m,
                Status = "Approved",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = entity.ToDomain();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result!.Id);
            Assert.Equal(entity.SourceAccountId, result.SourceAccountId);
            Assert.Equal(entity.TargetAccountId, result.TargetAccountId);
            Assert.Equal(entity.TransferTypeId, result.TransferTypeId);
            Assert.Equal(entity.Value, result.Value);
            Assert.Equal(TransactionStatus.Approved, result.Status);
            Assert.Equal(entity.CreatedAt, result.CreatedAt);
        }

        [Fact]
        public void ToDomain_ShouldDefaultToPending_WhenStatusIsInvalid()
        {
            // Arrange
            var entity = new TransactionEntity
            {
                Id = Guid.NewGuid(),
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid(),
                TransferTypeId = 1,
                Value = 100.50m,
                Status = "InvalidStatus",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = entity.ToDomain();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TransactionStatus.Pending, result!.Status);
        }

        [Fact]
        public void ToEntity_ShouldMapDomainToEntityCorrectly()
        {
            // Arrange
            var domain = new Transaction
            {
                Id = Guid.NewGuid(),
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid(),
                TransferTypeId = 1,
                Value = 200.75m,
                Status = TransactionStatus.Rejected,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = domain.ToEntity();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(domain.Id, result.Id);
            Assert.Equal(domain.SourceAccountId, result.SourceAccountId);
            Assert.Equal(domain.TargetAccountId, result.TargetAccountId);
            Assert.Equal(domain.TransferTypeId, result.TransferTypeId);
            Assert.Equal(domain.Value, result.Value);
            Assert.Equal(domain.Status.ToString(), result.Status);
            Assert.Equal(domain.CreatedAt, result.CreatedAt);
        }
    }

}
