using Confluent.Kafka;

namespace OrderService.Core;

public static class CoreExtensions
{
    public static IServiceCollection AddKafkaProducers(this IServiceCollection services, IConfigurationSection kafkaConfig)
    {
        services.AddSingleton<IProducer<string, string>>(_ =>
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = kafkaConfig["BootstrapServers"],
                Acks = Enum.Parse<Acks>(kafkaConfig["Produce:Acks"] ?? Acks.All.ToString()),
                BatchSize = int.Parse(kafkaConfig["Produce:BatchSize"] ?? "16384"),
                LingerMs = int.Parse(kafkaConfig["Produce:LingerMs"] ?? "5"),
            };
            return new ProducerBuilder<string, string>(producerConfig).Build();
        });

        return services;
    }
}