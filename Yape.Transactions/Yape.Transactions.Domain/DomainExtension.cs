using Microsoft.Extensions.DependencyInjection;
using Yape.Transactions.Domain.Transaction;
using Yape.Transactions.Domain.Transaction.portsIn;


namespace Yape.Transactions.Domain
{
    public static class DomainExtension
    {
        public static IServiceCollection AddDomain(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITransactionService, TransactionService>();
          
            return serviceCollection;
        }
    }
}
