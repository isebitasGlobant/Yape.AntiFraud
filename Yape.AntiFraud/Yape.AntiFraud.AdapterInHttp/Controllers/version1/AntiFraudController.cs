
using Microsoft.AspNetCore.Mvc;
using Yape.AntiFraud.AdapterInHttp.DTOs;
using Yape.AntiFraud.AdapterInHttp.Mappers;
using Yape.AntiFraud.Domain.Transaction.portsIn;

namespace Yape.AntiFraud.AdapterInHttp.Controllers.version1
{
    /// <summary>
    /// Controller for handling Anti-Fraud related operations.
    /// </summary>
    [ApiController]
    [Route("api/v1/antifraud")]
    public class AntiFraudController : ControllerBase
    {
        private readonly IAntiFraudService _antiFraudService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AntiFraudController"/> class.
        /// </summary>
        /// <param name="antiFraudService">Service for Anti-Fraud operations.</param>
        public AntiFraudController(IAntiFraudService antiFraudService)
        {
            _antiFraudService = antiFraudService;
        }

        /// <summary>
        /// Validates a transaction based on the provided request.
        /// </summary>
        /// <param name="request">The transaction update message request.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
        [HttpPost("validate")]
        public async Task<IActionResult> ValidateTransaction([FromBody] TransactionUpdateMessageRequest request)
        {
            _ = await _antiFraudService.ValidateTransaction(request.ToDomain());
            return Ok();
        }
    }

}
