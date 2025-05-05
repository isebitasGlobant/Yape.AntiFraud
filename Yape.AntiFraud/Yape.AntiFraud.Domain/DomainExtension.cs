using Microsoft.Extensions.DependencyInjection;
using Yape.AntiFraud.Domain.Transaction;
using Yape.AntiFraud.Domain.Transaction.portsIn;


namespace Yape.AntiFraud.Domain
{
    public static class DomainExtension
    {
        public static IServiceCollection AddDomain(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IAntiFraudService, AntiFraudService>();
          
            return serviceCollection;
        }
    }
}
