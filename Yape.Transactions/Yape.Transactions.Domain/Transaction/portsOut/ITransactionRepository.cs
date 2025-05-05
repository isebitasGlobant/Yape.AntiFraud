using Yape.Transactions.Domain.Transaction.models;

namespace Yape.Transactions.Domain.Transaction.portsOut
{
    public interface ITransactionRepository
    {
        Task<models.Transaction> GetById(Guid id);
        Task SaveAsync(models.Transaction transaction);
        Task UpdateStatusAsync(models.Transaction transaction);
    }
}
