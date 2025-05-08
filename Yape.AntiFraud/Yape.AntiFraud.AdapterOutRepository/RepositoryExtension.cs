using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Yape.AntiFraud.AdapterOutRepository.postgreSql.repositories;
using Yape.AntiFraud.Domain.Transaction.portsOut;

namespace Yape.AntiFraud.AdapterOutRepository
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
