
using Yape.AntiFraud.AdapterInHttp.DTOs;
using Yape.AntiFraud.AdapterInHttp.Mappers;
using Yape.AntiFraud.Domain.Transaction.models;

namespace Yape.AntiFraud.AdapterInHttp.Tests.Mappers
{
    public class TransactionMapperTests
    {
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
