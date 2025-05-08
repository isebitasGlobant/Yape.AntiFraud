using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Yape.Transactions.AdapterOutRepository.postgreSql.Repositories;
using Yape.Transactions.Domain.Transaction.portsOut;

namespace Yape.Transactions.AdapterOutRepository
{
    [ExcludeFromCodeCoverage]
    public static class RepositoryExtension
    {
        public static IServiceCollection AddRepository(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITransactionRepository, TransactionRepository>();

            return serviceCollection;
        }
    }
}
