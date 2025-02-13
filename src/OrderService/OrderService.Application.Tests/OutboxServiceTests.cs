using Confluent.Kafka;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Abstractions;
using OrderService.Entities.Models.Responses;
using OrderService.Fixture;

namespace OrderService.Application.Tests;

public class OutboxServiceTests(TestContainersFixture fixture) : IClassFixture<TestContainersFixture>
{
    private readonly ApplicationFixtureFactory _factory = new(fixture.PostgresConnectionString, fixture.KafkaBootstrapServer);

    [Fact(DisplayName = "ProcessAsync should send messages to kafka")]
    public async Task ProcessAsync_ShouldSendMessagesToKafka()
    {
        // Arrange
        const int seedMessagesCount = 10;
        var scope = _factory.CreateScope();
        var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();
        var consumer = scope.ServiceProvider.GetRequiredService<IConsumer<Guid, OutboxResponseModel>>();

        // Act
        consumer.Subscribe("order-outbox-service");
        await outboxService.ProcessAsync();

        var receivedMessages = new List<OutboxResponseModel>();

        for (var i = 0; i < seedMessagesCount; i++)
        {
            var consumerResult = consumer.Consume(TimeSpan.FromSeconds(10));
            receivedMessages.Add(consumerResult.Message.Value);
        }

        // Assert
        receivedMessages.Should().NotBeNull().And.HaveCount(seedMessagesCount);
    }

}