namespace Yape.AntiFraud.Domain.Transaction.portsIn
{
    public interface IAntiFraudService
    {
        Task<models.Transaction> ValidateTransaction(models.Transaction request);
    }
}
