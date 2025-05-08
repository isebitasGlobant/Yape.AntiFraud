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
            services.AddSingleton(new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            });

            services.AddSingleton<IProducer<string, string>>(provider =>
            {
                var producerConfig = provider.GetRequiredService<ProducerConfig>();
                return new ProducerBuilder<string, string>(producerConfig).Build();
            });

            services.AddSingleton<IKafkaProducer, KafkaProducer>();
            services.AddScoped<IEventPublisher, KafkaEventPublisher>();
            services.AddSingleton<KafkaConsumerService>();
            return services;
        }
    }
}
