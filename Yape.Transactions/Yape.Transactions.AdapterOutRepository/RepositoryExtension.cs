using Microsoft.Extensions.DependencyInjection;
using Yape.AntiFraud.AdapterOutRepository.postgreSql.repositories;
using Yape.Transactions.Domain.Transaction.portsOut;

namespace Yape.AntiFraud.AdapterOutRepository
{
    public static class RepositoryExtension
    {
        public static IServiceCollection AddRepository(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITransactionRepository, TransactionRepository>();

            return serviceCollection;
        }
    }
}
