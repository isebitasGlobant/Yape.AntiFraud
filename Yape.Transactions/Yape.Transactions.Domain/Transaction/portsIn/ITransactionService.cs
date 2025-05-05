
namespace Yape.Transactions.Domain.Transaction.portsIn
{
    public interface ITransactionService
    {
        Task<models.Transaction> CreateTransactionAsync(models.Transaction request);
        Task<models.Transaction> UpdateTransactionAsync(models.Transaction request);
        Task<models.Transaction> GetTransactionByIdAsync(Guid id);
    }
}
