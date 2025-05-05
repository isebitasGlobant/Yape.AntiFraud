namespace Yape.AntiFraud.Domain.Transaction.portsOut
{
    public interface ITransactionRepository
    {
        Task<models.Transaction> GetByIdAsync(Guid id);
        Task<List<models.Transaction>> GetTotalAmountForTodaysBySourceAccountIdAsync(Guid SourceAccountId);
        Task SaveAsync(models.Transaction transaction);
        Task UpdateStatusAsync(models.Transaction transaction);
    }
}
