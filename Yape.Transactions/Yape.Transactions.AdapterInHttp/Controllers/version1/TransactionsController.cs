
using Microsoft.AspNetCore.Mvc;
using Yape.Transactions.AdapterInHttp.DTOs;
using Yape.Transactions.AdapterInHttp.Mappers;
using Yape.Transactions.Domain.Transaction.portsIn;
using Serilog;
using Yape.Transactions.Domain.Transaction.models;

namespace Yape.Transactions.AdapterInHttp.Controllers.version1
{
    [ApiController]
    [Route("api/v1/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _antiFraudService;

        public TransactionsController(ITransactionService antiFraudService)
        {
            _antiFraudService = antiFraudService;
        }

        /// <summary>
        /// Creates a new transaction.
        /// </summary>
        /// <param name="request">The transaction request.</param>
        /// <returns>The created transaction.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionRequest request)
        {
            // This must be improbe with FluentValidation
            if (request.SourceAccountId == Guid.Empty)
            {
                return BadRequest($"Parameter {nameof(request.SourceAccountId)} canot be empty.");
            }
            if (request.TargetAccountId == Guid.Empty)
            {
                return BadRequest($"Parameter {nameof(request.TargetAccountId)} canot be empty.");
            }
            if (request.TransferTypeId <= 0)
            {
                return BadRequest($"Parameter {nameof(request.TransferTypeId)} canot be empty.");
            }
            if (request.Value <= 0)
            {
                return BadRequest($"Parameter {nameof(request.Value)} canot be empty.");
            }
            if (request.SourceAccountId == request.TargetAccountId)
            {
                return BadRequest($"Parameter {nameof(request.SourceAccountId)} and {nameof(request.TargetAccountId)} cannot be the same.");
            }

            var transaction = await _antiFraudService.CreateTransactionAsync(request.ToDomain());
            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
        }

        /// <summary>
        /// Retrieves a transaction by its ID.
        /// </summary>
        /// <param name="id">The transaction ID.</param>
        /// <returns>The transaction details.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransaction(Guid id)
        {
            // This must be improbe with FluentValidation
            if (id == Guid.Empty)
            {
                return BadRequest("The id must be valid.");
            }

            var transaction = await _antiFraudService.GetTransactionByIdAsync(id);
            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }

        [HttpPost("update-transaction")]
        public async Task<IActionResult> UpdateTransactionAsync([FromBody] TransactionUpdateMessageRequest request)
        {
            if (request.Id == Guid.Empty)
            {
                return BadRequest($"Parameter {nameof(request.Id)} must be valid.");
            }

            if (!Enum.TryParse<TransactionStatus>(request.Status, out var status))
            {
                return BadRequest($"Parameter {nameof(request.Status)} Invalid transaction status.");
            }

            var transaction = await _antiFraudService.UpdateTransactionAsync(request.ToDomain());

            Log.Information("Updating transaction: {TransactionId} with status {Status}", request.Id, request.Status);
            return Ok();
        }
    }

}
