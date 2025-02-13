using Confluent.Kafka;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.KafkaSerializers;
using OrderService.Application.Tests.TestKafkaDeserializers;
using OrderService.Entities.Models.Responses;
using OrderService.Fixture;
using OrderService.Mapping;
using OrderService.Postgres;
using OrderService.Repositories.Helpers;

namespace OrderService.Application.Tests;

public class ApplicationFixtureFactory(string connectionString, string kafkaBootstrapServer)
{
    private readonly ApplicationFixtureFactoryImpl _privateFactory = new(connectionString, kafkaBootstrapServer);

    public IServiceScope CreateScope() => _privateFactory.CreateScopeInternal();

    private class ApplicationFixtureFactoryImpl(string connectionString, string kafkaBootstrapServer) :
        FixtureFactoryBase(connectionString)
    {
        private readonly string _connectionString = connectionString;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            var outboxConfig = new OutboxConfig { BatchSize = 10 };

            builder.ConfigureServices(services =>
            {
                services
                    .AddSingleton(outboxConfig)
                    .AddRepositoriesModule()
                    .AddPostgresMigrations(_connectionString)
                    .AddMappingModule()
                    .AddApplicationServicesModule();

                AddKafkaProducer(services);
                AddKafkaConsumer(services);
            });
        }

        private void AddKafkaProducer(IServiceCollection services)
        {
            services.AddSingleton<IProducer<Guid, OutboxResponseModel>>(_ =>
            {
                var producerConfig = new ProducerConfig
                {
                    BootstrapServers = kafkaBootstrapServer
                };
                return new ProducerBuilder<Guid, OutboxResponseModel>(producerConfig)
                    .SetKeySerializer(new GuidSerializer())
                    .SetValueSerializer(new OutboxResponseModelSerializer())
                    .Build();
            });
        }

        private void AddKafkaConsumer(IServiceCollection services)
        {
            services.AddSingleton<IConsumer<Guid, OutboxResponseModel>>(_ =>
            {
                var consumerConfig = new ConsumerConfig
                {
                    BootstrapServers = kafkaBootstrapServer,
                    GroupId = "testOutbox-consumer-group",
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };
                return new ConsumerBuilder<Guid, OutboxResponseModel>(consumerConfig)
                    .SetKeyDeserializer(new GuidDeserializer())
                    .SetValueDeserializer(new OutboxResponseModelDeserializer())
                    .Build();
            });
        }
    }
}