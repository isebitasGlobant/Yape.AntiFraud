
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Yape.AntiFraud.AdapterOutRepository.postgreSql.entities;

namespace Yape.AntiFraud.AdapterOutRepository.postgreSql
{
    public class PostgreSqlContext : DbContext
    {
        private IDbContextTransaction contextTransaction;

        public PostgreSqlContext(DbContextOptions<PostgreSqlContext> options) : base(options) { }

        public virtual DbSet<TransactionEntity> Transactions { get; set; }
    }
}
