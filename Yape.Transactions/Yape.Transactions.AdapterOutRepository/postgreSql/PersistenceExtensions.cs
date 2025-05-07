using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Yape.Transactions.AdapterOutRepository.postgreSql
{
    [ExcludeFromCodeCoverage]
    public static class PersistenceExtensions
    {
        public static void AddPersistence(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddDbContext<PostgreSqlContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("YapePostgresDb")));
        }

        public static void MigrateDatabase(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();

            using var context = serviceScope.ServiceProvider.GetService<PostgreSqlContext>();
            context?.Database.Migrate();
        }
    }
}
