
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Yape.Transactions.AdapterOutRepository.postgreSql.entities;

namespace Yape.Transactions.AdapterOutRepository.postgreSql
{
    public class PostgreSqlContext : DbContext
    {
        private IDbContextTransaction contextTransaction;

        public PostgreSqlContext(DbContextOptions<PostgreSqlContext> options) : base(options) { }

        public virtual DbSet<TransactionEntity> Transactions { get; set; }
    }
}
