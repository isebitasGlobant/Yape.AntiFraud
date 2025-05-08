using Microsoft.AspNetCore.Mvc;
using Moq;
using Yape.Transactions.AdapterInHttp.Controllers.version1;
using Yape.Transactions.AdapterInHttp.DTOs;
using Yape.Transactions.Domain.Transaction.models;
using Yape.Transactions.Domain.Transaction.portsIn;

namespace Yape.AntiFraud.AdapterInHttp.Tests.Controllers.version1
{
    public class TransactionsControllerTests
    {
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly TransactionsController _controller;

        public TransactionsControllerTests()
        {
            _mockTransactionService = new Mock<ITransactionService>();
            _controller = new TransactionsController(_mockTransactionService.Object);
        }

        [Fact]
        public async Task CreateTransaction_ReturnsBadRequest_WhenSourceAccountIdIsEmpty()
        {
            // Arrange
            var request = new TransactionRequest
            {
                SourceAccountId = Guid.Empty,
                TargetAccountId = Guid.NewGuid(),
                TransferTypeId = 1,
                Value = 100.00m
            };

            // Act
            var result = await _controller.CreateTransaction(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"Parameter {nameof(request.SourceAccountId)} canot be empty.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateTransaction_ReturnsBadRequest_WhenTargetAccountIdIsEmpty()
        {
            // Arrange
            var request = new TransactionRequest
            {
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.Empty,
                TransferTypeId = 1,
                Value = 100.00m
            };

            // Act
            var result = await _controller.CreateTransaction(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"Parameter {nameof(request.TargetAccountId)} canot be empty.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateTransaction_ReturnsBadRequest_WhenTransferTypeIdIsInvalid()
        {
            // Arrange
            var request = new TransactionRequest
            {
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid(),
                TransferTypeId = 0,
                Value = 100.00m
            };

            // Act
            var result = await _controller.CreateTransaction(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"Parameter {nameof(request.TransferTypeId)} canot be empty.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateTransaction_ReturnsBadRequest_WhenValueIsInvalid()
        {
            // Arrange
            var request = new TransactionRequest
            {
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid(),
                TransferTypeId = 1,
                Value = 0
            };

            // Act
            var result = await _controller.CreateTransaction(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"Parameter {nameof(request.Value)} canot be empty.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateTransaction_ReturnsBadRequest_WhenSourceAndTargetAccountIdsAreSame()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var request = new TransactionRequest
            {
                SourceAccountId = accountId,
                TargetAccountId = accountId,
                TransferTypeId = 1,
                Value = 100.00m
            };

            // Act
            var result = await _controller.CreateTransaction(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"Parameter {nameof(request.SourceAccountId)} and {nameof(request.TargetAccountId)} cannot be the same.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetTransaction_ReturnsBadRequest_WhenIdIsEmpty()
        {
            // Arrange
            var transactionId = Guid.Empty;

            // Act
            var result = await _controller.GetTransaction(transactionId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The id must be valid.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateTransactionAsync_ReturnsBadRequest_WhenIdIsEmpty()
        {
            // Arrange
            var request = new TransactionUpdateMessageRequest
            {
                Id = Guid.Empty,
                Status = "Approved"
            };

            // Act
            var result = await _controller.UpdateTransactionAsync(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"Parameter {nameof(request.Id)} must be valid.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateTransactionAsync_ReturnsBadRequest_WhenStatusIsInvalid()
        {
            // Arrange
            var request = new TransactionUpdateMessageRequest
            {
                Id = Guid.NewGuid(),
                Status = "InvalidStatus"
            };

            // Act
            var result = await _controller.UpdateTransactionAsync(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"Parameter {nameof(request.Status)} Invalid transaction status.", badRequestResult.Value);
        }
    }
}
