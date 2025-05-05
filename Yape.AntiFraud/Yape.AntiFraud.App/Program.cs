using Serilog;
using Yape.AntiFraud.AdapterInHttp;
using Yape.AntiFraud.AdapterOutKafka;
using Yape.AntiFraud.AdapterOutRepository;
using Yape.AntiFraud.AdapterOutRepository.postgreSql;
using Yape.AntiFraud.Domain;

namespace Yape.AntiFraud.App
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
                Log.Information("Starting up the application AntiFraud");

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

                builder.Services.AddSwaggerGenCustomized("Reto AntiFraud");

                builder.Services.ConfigureVersioning();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                app.AllowSwaggerToListApiVersions("Reto AntiFraud");
                app.AddXMLDocumentation();

                app.UseHttpsRedirection();

                app.UseAuthorization();

                app.MapControllers();

                app.MigrateDatabase();

                // Initialize Kafka Consumer (This should be move to another microservice in order to centralize the logic for distributions across multiple microservices)
                var kafkaConsumer = app.Services.GetRequiredService<KafkaConsumerService>();
                Task.Factory.StartNew(() =>
                {
                    Log.Information("Starting Kafka consumer for topic: TransactionCreated");
                    kafkaConsumer.StartConsuming(topic: "TransactionCreated", "https://localhost:7124/api/v1/antifraud/validate");
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