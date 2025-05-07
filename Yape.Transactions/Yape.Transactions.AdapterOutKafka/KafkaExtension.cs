using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Yape.Transactions.AdapterOutKafka.Client;
using Yape.Transactions.AdapterOutKafka.Client.Contracts;

namespace Yape.Transactions.AdapterOutKafka
{
    [ExcludeFromCodeCoverage]
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
            services.AddSingleton<IKafkaProducer, KafkaProducer>();
            services.AddSingleton<KafkaConsumerService>();
            return services;
        }
    }
}
