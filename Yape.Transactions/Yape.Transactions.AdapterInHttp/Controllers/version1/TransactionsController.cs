
using Microsoft.AspNetCore.Mvc;
using Yape.Transactions.AdapterInHttp.DTOs;
using Yape.Transactions.AdapterInHttp.Mappers;
using Yape.Transactions.Domain.Transaction.portsIn;
using Serilog;

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
            var transaction = await _antiFraudService.GetTransactionByIdAsync(id);
            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }

        [HttpPost("update-transaction")]
        public async Task<IActionResult> UpdateTransactionAsync([FromBody] TransactionUpdateMessageRequest request)
        {
            var transaction = await _antiFraudService.UpdateTransactionAsync(request.ToDomain());

            Log.Information("Updating transaction: {TransactionId} with status {Status}", request.Id, request.Status);
            return Ok();
        }
    }

}
