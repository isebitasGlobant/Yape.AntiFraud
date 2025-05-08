
using Yape.AntiFraud.Domain.Transaction.models;
using Yape.AntiFraud.Domain.Transaction.portsIn;
using Yape.AntiFraud.Domain.Transaction.portsOut;

namespace Yape.AntiFraud.Domain.Transaction
{
    public class AntiFraudService : IAntiFraudService
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly ITransactionRepository _transactionRepository;
        private string Topic = "TransactionValidated"; // Replace with your actual topic name

        public AntiFraudService(IEventPublisher eventPublisher, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task<models.Transaction> ValidateTransaction(models.Transaction transaction)
        {
            var storedTransaction = await _transactionRepository.GetByIdAsync(transaction.Id);

            if (storedTransaction.Value > 2000 && storedTransaction.Status == TransactionStatus.Pending)
            {
                storedTransaction.Status = TransactionStatus.Rejected;
                await _eventPublisher.PublishAsync(Topic, storedTransaction);
            }
            else if (storedTransaction.Value < 2000 && storedTransaction.Status == TransactionStatus.Pending)
            {
                var total = await _transactionRepository.GetTotalAmountForTodaysBySourceAccountIdAsync(storedTransaction.SourceAccountId);
                if (total.Where(t => t.Status == TransactionStatus.Approved).Sum(t => t.Value) > 20000)
                {
                    storedTransaction.Status = TransactionStatus.Rejected;
                    await _eventPublisher.PublishAsync(Topic, storedTransaction);
                }
                else
                {
                    storedTransaction.Status = TransactionStatus.Approved;
                    await _eventPublisher.PublishAsync(Topic, storedTransaction);
                }

            }
            else
            {
                storedTransaction.Status = TransactionStatus.Approved;
                await _eventPublisher.PublishAsync(Topic, storedTransaction);
            }
            return storedTransaction;
        }
    }

}
