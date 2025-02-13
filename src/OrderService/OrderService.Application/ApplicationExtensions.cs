using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Abstractions;
using OrderService.Application.KafkaSerializers;
using OrderService.Entities.Models.Responses;

namespace OrderService.Application;

public static class ApplicationExtensions
{
    /// <summary>
    /// Adds module with domain services
    /// </summary>
    /// <param name="services">Service collection</param>
    public static IServiceCollection AddApplicationServicesModule(this IServiceCollection services)
    {
        return services.AddScoped<IOrderService, OrderService>()
            .AddScoped<IOutboxService, OutboxService>();
    }

    public static IServiceCollection RegisterOutboxConfig(this IServiceCollection services, IConfigurationSection outboxSection)
    {
        var dbConfig = new OutboxConfig();
        outboxSection.Bind(dbConfig);
        services.AddSingleton(dbConfig);

        return services;
    }

    /// <summary>
    /// Adds kafka producers to DI
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="kafkaConfig">Configuration section</param>
    public static IServiceCollection AddKafkaProducers(this IServiceCollection services, IConfigurationSection kafkaConfig)
    {
        services.AddSingleton<IProducer<Guid, OutboxResponseModel>>(_ =>
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = kafkaConfig["BootstrapServers"],
                Acks = Enum.Parse<Acks>(kafkaConfig["Produce:Acks"] ?? Acks.All.ToString()),
                BatchSize = int.Parse(kafkaConfig["Produce:BatchSize"] ?? "16384"),
                LingerMs = int.Parse(kafkaConfig["Produce:LingerMs"] ?? "5"),
            };
            return new ProducerBuilder<Guid, OutboxResponseModel>(producerConfig)
                .SetKeySerializer(new GuidSerializer())
                .SetValueSerializer(new OutboxResponseModelSerializer())
                .Build();
        });

        return services;
    }
}