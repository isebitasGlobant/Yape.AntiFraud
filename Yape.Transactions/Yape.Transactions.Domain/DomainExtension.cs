using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Yape.Transactions.Domain.Transaction;
using Yape.Transactions.Domain.Transaction.portsIn;


namespace Yape.Transactions.Domain
{
    [ExcludeFromCodeCoverage]
    public static class DomainExtension
    {
        public static IServiceCollection AddDomain(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITransactionService, TransactionService>();
          
            return serviceCollection;
        }
    }
}
