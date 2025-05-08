
using Yape.Transactions.AdapterInHttp.DTOs;
using Yape.Transactions.AdapterInHttp.Mappers;
using Yape.Transactions.Domain.Transaction.models;

namespace Yape.AntiFraud.AdapterInHttp.Tests.Mappers
{
    public class TransactionMapperTests
    {
        [Fact]
        public void ToDomain_ShouldMapTransactionRequestToTransaction()
        {
            // Arrange
            var request = new TransactionRequest
            {
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid(),
                TransferTypeId = 1,
                Value = 100.50m
            };

            // Act
            var result = request.ToDomain();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.SourceAccountId, result.SourceAccountId);
            Assert.Equal(request.TargetAccountId, result.TargetAccountId);
            Assert.Equal(request.TransferTypeId, result.TransferTypeId);
            Assert.Equal(request.Value, result.Value);
        }

        [Fact]
        public void ToDomain_ShouldMapTransactionUpdateMessageRequestToTransaction_WithValidStatus()
        {
            // Arrange
            var request = new TransactionUpdateMessageRequest
            {
                Id = Guid.NewGuid(),
                Status = "Approved"
            };

            // Act
            var result = request.ToDomain();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Id, result.Id);
            Assert.Equal(TransactionStatus.Approved, result.Status);
        }

        [Fact]
        public void ToDomain_ShouldMapTransactionUpdateMessageRequestToTransaction_WithInvalidStatus()
        {
            // Arrange
            var request = new TransactionUpdateMessageRequest
            {
                Id = Guid.NewGuid(),
                Status = "InvalidStatus"
            };

            // Act
            var result = request.ToDomain();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Id, result.Id);
            Assert.Equal(TransactionStatus.Pending, result.Status); // Default to Pending
        }
    }
}
