using Serilog;
using Yape.AntiFraud.AdapterOutRepository;
using Yape.AntiFraud.AdapterOutRepository.postgreSql;
using Yape.Transactions.AdapterInHttp;
using Yape.Transactions.AdapterOutKafka;
using Yape.Transactions.AdapterOutKafka.Client;
using Yape.Transactions.Domain;

namespace Yape.Transactions.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Starting up the application Transactions");

                IConfiguration configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json")
                                .AddEnvironmentVariables()
                                .Build();

                var builder = WebApplication.CreateBuilder(args);

                // Add Serilog to the logging pipeline
                builder.Host.UseSerilog();

                // Add services to the container.
                builder.Services.AddControllers();
                builder.Services.AddHttpClient();

                builder.Services.AddKafka(configuration);
                builder.Services.AddDomain();
                builder.Services.AddPersistence(configuration);

                builder.Services.AddRepository();

                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();

                builder.Services.AddSwaggerGenCustomized("Reto Transactions");

                builder.Services.ConfigureVersioning();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                app.AllowSwaggerToListApiVersions("Reto Transactions");
                app.AddXMLDocumentation();

                app.UseHttpsRedirection();

                app.UseAuthorization();

                app.MapControllers();

                app.MigrateDatabase();

                // Initialize Kafka Consumer (This should be move to another microservice in order to centralize the logic for distributions across multiple microservices)
                var kafkaConsumer = app.Services.GetRequiredService<KafkaConsumerService>();
                Task.Factory.StartNew(() =>
                {
                    Log.Information("Starting Kafka consumer for topic: TransactionValidated");
                    kafkaConsumer.StartConsuming(topic: "TransactionValidated", "https://localhost:7125/api/v1/transactions/update-transaction");
                }, TaskCreationOptions.LongRunning);

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application startup failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}