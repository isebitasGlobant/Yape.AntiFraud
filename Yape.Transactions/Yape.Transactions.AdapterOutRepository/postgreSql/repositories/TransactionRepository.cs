using Yape.Transactions.AdapterOutRepository.postgreSql.Mapper;
using Yape.Transactions.Domain.Transaction.models;
using Yape.Transactions.Domain.Transaction.portsOut;

namespace Yape.Transactions.AdapterOutRepository.postgreSql.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly PostgreSqlContext _context;

        public TransactionRepository(PostgreSqlContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetById(Guid id)
        {
            var result = await _context.Transactions.FindAsync(id);
            return result?.ToDomain();
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
