using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yape.Transactions.AdapterOutKafka.Client;

namespace Yape.Transactions.AdapterOutKafka
{
    public static class KafkaExtension
    {
        public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                GroupId = configuration["Kafka:GroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            });
            services.AddScoped<IEventPublisher, KafkaEventPublisher>();
            services.AddSingleton<KafkaProducer>();
            services.AddSingleton<KafkaConsumerService>();
            return services;
        }
    }
}
