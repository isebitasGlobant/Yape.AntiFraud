using Microsoft.EntityFrameworkCore;
using Yape.AntiFraud.AdapterOutRepository.postgreSql.Mapper;

using Yape.AntiFraud.Domain.Transaction.models;
using Yape.AntiFraud.Domain.Transaction.portsOut;

namespace Yape.AntiFraud.AdapterOutRepository.postgreSql.repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly PostgreSqlContext _context;

        public TransactionRepository(PostgreSqlContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetByIdAsync(Guid id)
        {
            var result = await _context.Transactions.FindAsync(id);
            return result?.ToDomain();
        }

        public async Task<List<Transaction>> GetTotalAmountForTodaysBySourceAccountIdAsync(Guid SourceAccountId)
        {
            var result = await _context.Transactions
                .Where(t => t.SourceAccountId == SourceAccountId && t.CreatedAt.Date == DateTime.UtcNow.Date)
                .ToListAsync();

            return result.Any()? result.Select(t => t.ToDomain()).ToList() : new List<Transaction>();
        }

        public async Task SaveAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction.ToEntity());
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(Transaction transaction)
        {
            var existingTransaction = await _context.Transactions.FindAsync(transaction.Id);
            if (existingTransaction != null)
            {
                existingTransaction.Status = transaction.Status.ToString();
                await _context.SaveChangesAsync();
            }
        }

    }
}
