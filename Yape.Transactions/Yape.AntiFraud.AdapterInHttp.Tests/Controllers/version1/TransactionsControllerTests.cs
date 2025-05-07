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
        public async Task CreateTransaction_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var request = new TransactionRequest
            {
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid(),
                TransferTypeId = 1,
                Value = 100.00m
            };

            var createdTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                SourceAccountId = request.SourceAccountId,
                TargetAccountId = request.TargetAccountId,
                TransferTypeId = request.TransferTypeId,
                Value = request.Value,
                CreatedAt = DateTime.UtcNow,
                Status = TransactionStatus.Pending
            };

            _mockTransactionService
                .Setup(service => service.CreateTransactionAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(createdTransaction);

            // Act
            var result = await _controller.CreateTransaction(request);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetTransaction), createdAtActionResult.ActionName);
            Assert.Equal(createdTransaction, createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetTransaction_ReturnsOkObjectResult_WhenTransactionExists()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var transaction = new Transaction
            {
                Id = transactionId,
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid(),
                TransferTypeId = 1,
                Value = 100.00m,
                CreatedAt = DateTime.UtcNow,
                Status = TransactionStatus.Pending
            };

            _mockTransactionService
                .Setup(service => service.GetTransactionByIdAsync(transactionId))
                .ReturnsAsync(transaction);

            // Act
            var result = await _controller.GetTransaction(transactionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(transaction, okResult.Value);
        }

        [Fact]
        public async Task GetTransaction_ReturnsNotFoundResult_WhenTransactionDoesNotExist()
        {
            // Arrange
            var transactionId = Guid.NewGuid();

            _mockTransactionService
                .Setup(service => service.GetTransactionByIdAsync(transactionId))
                .ReturnsAsync((Transaction)null);

            // Act
            var result = await _controller.GetTransaction(transactionId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateTransactionAsync_ReturnsOkResult()
        {
            // Arrange
            var request = new TransactionUpdateMessageRequest
            {
                Id = Guid.NewGuid(),
                Status = "Approved"
            };

            var updatedTransaction = new Transaction
            {
                Id = request.Id,
                Status = TransactionStatus.Approved
            };

            _mockTransactionService
                .Setup(service => service.UpdateTransactionAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(updatedTransaction);

            // Act
            var result = await _controller.UpdateTransactionAsync(request);

            // Assert
            Assert.IsType<OkResult>(result);
        }
    }
}
