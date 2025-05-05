using Microsoft.Extensions.Logging;
using Yape.Transactions.Domain.Transaction.models;
using Yape.Transactions.Domain.Transaction.portsIn;
using Yape.Transactions.Domain.Transaction.portsOut;

namespace Yape.Transactions.Domain.Transaction
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(ITransactionRepository transactionRepository, IEventPublisher eventPublisher, ILogger<TransactionService> logger)
        {
            _transactionRepository = transactionRepository;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<models.Transaction> CreateTransactionAsync(models.Transaction transaction)
        {
            _logger.LogInformation("Creating a new transaction with SourceAccountId: {SourceAccountId} and TargetAccountId: {TargetAccountId}", transaction.SourceAccountId, transaction.TargetAccountId);

            

            // Save transaction with pending status
            await _transactionRepository.SaveAsync(transaction);

            _logger.LogInformation("Transaction saved with ID: {TransactionId}", transaction.Id);

            // Publish event to Kafka
            await _eventPublisher.PublishAsync("TransactionCreated", transaction);

            _logger.LogInformation("TransactionCreated event published for Transaction ID: {TransactionId}", transaction.Id);

            return transaction;
        }

        public async Task<models.Transaction> GetTransactionByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching transaction with ID: {TransactionId}", id);

            var existingTransaction = await _transactionRepository.GetById(id);

            if (existingTransaction == null)
            {
                _logger.LogWarning("Transaction with ID: {TransactionId} not found", id);
            }

            return existingTransaction;
        }

        public async Task<models.Transaction> UpdateTransactionAsync(models.Transaction request)
        {
            _logger.LogInformation("Updating transaction with ID: {TransactionId} to Status: {Status}", request.Id, request.Status);

            if (request.Status == TransactionStatus.Approved)
            {
                var storedTransaction = await _transactionRepository.GetById(request.Id);
                if (storedTransaction != null)
                {
                    storedTransaction.Status = request.Status;
                    await _transactionRepository.UpdateStatusAsync(storedTransaction);

                    _logger.LogInformation("Transaction with ID: {TransactionId} updated to Status: {Status}", storedTransaction.Id, storedTransaction.Status);

                    return storedTransaction;
                }
                else
                {
                    _logger.LogError("Transaction with ID: {TransactionId} not found", request.Id);
                    throw new Exception("Transaction not found");
                }
            }

            return request;
        }
    }

}
