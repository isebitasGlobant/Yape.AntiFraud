using Microsoft.AspNetCore.Mvc;
using Moq;
using Yape.AntiFraud.AdapterInHttp.Controllers.version1;
using Yape.AntiFraud.AdapterInHttp.DTOs;
using Yape.AntiFraud.Domain.Transaction.models;
using Yape.AntiFraud.Domain.Transaction.portsIn;

namespace Yape.AntiFraud.AdapterInHttp.Tests.Controllers.version1
{
    public class AntiFraudControllerTests
    {
        private readonly Mock<IAntiFraudService> _antiFraudServiceMock;
        private readonly AntiFraudController _controller;

        public AntiFraudControllerTests()
        {
            _antiFraudServiceMock = new Mock<IAntiFraudService>();
            _controller = new AntiFraudController(_antiFraudServiceMock.Object);
        }

        [Fact]
        public async Task ValidateTransaction_ReturnsOk_WhenValidationIsSuccessful()
        {
            // Arrange
            var request = new TransactionUpdateMessageRequest
            {
                Id = Guid.NewGuid(),
                Status = "Pending"
            };

            var domainTransaction = new Transaction
            {
                Id = request.Id,
                Status = TransactionStatus.Pending
            };

            _antiFraudServiceMock
                .Setup(service => service.ValidateTransaction(It.IsAny<Transaction>()))
                .ReturnsAsync(domainTransaction);

            // Act
            var result = await _controller.ValidateTransaction(request);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _antiFraudServiceMock.Verify(service => service.ValidateTransaction(It.Is<Transaction>(t => t.Id == request.Id)), Times.Once);
        }

        [Fact]
        public async Task ValidateTransaction_ReturnsBadRequest_WhenRequestIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Id", "The Id field is required.");

            var request = new TransactionUpdateMessageRequest
            {
                Id = Guid.Empty,
                Status = "Invalid"
            };

            // Act
            var result = await _controller.ValidateTransaction(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _antiFraudServiceMock.Verify(service => service.ValidateTransaction(It.IsAny<Transaction>()), Times.Never);
        }

        [Fact]
        public async Task ValidateTransaction_ThrowsException_WhenServiceFails()
        {
            // Arrange
            var request = new TransactionUpdateMessageRequest
            {
                Id = Guid.NewGuid(),
                Status = "Pending"
            };

            _antiFraudServiceMock
                .Setup(service => service.ValidateTransaction(It.IsAny<Transaction>()))
                .ThrowsAsync(new Exception("Service failure"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.ValidateTransaction(request));
            _antiFraudServiceMock.Verify(service => service.ValidateTransaction(It.Is<Transaction>(t => t.Id == request.Id)), Times.Once);
        }
    }
}
