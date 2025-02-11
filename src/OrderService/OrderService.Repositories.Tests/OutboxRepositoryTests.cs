using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Entities;
using OrderService.Entities.Models.Entities;
using OrderService.Fixture;
using OrderService.Repositories.Abstractions;

namespace OrderService.Repositories.Tests;

public class OutboxRepositoryTests(TestContainersFixture testContainersFixture) : IClassFixture<TestContainersFixture>
{
    private readonly RepositoriesFixtureFactory _factory = new(testContainersFixture.PostgresConnectionString);

    [Theory(DisplayName = "AddAsync should insert new message into database"), AutoData]
    public async Task AddAsync_ShouldAddMessageToDatabase(OutboxMessage message)
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Act
        await uow.BeginTransactionAsync();
        var res = await repo.AddAsync(message, uow.Connection, uow.Transaction!);
        await uow.CommitAsync();

        // Assert
        res.Should().NotBeNull();
        res.Id.Should().Be(message.Id);
    }

    [Theory(DisplayName = "UpdateAsync should affect row in database"), AutoData]
    public async Task UpdateAsync_ShouldUpdateMessageInDatabase(OutboxMessage message)
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Act
        await uow.BeginTransactionAsync();
        var res = await repo.AddAsync(message, uow.Connection, uow.Transaction!);
        await uow.CommitAsync();

        await uow.BeginTransactionAsync();
        var isUpdated = await repo.UpdateAsync([res.Id], MessageStatus.Unknown2, uow.Connection, uow.Transaction!);

        // Arrange
        isUpdated.Should().BeTrue();
    }

    [Theory(DisplayName = "FetchMessagesByStatusAsync should return messages by status and set status as processing"),
     AutoData]
    public async Task FetchMessagesByStatusAsync_ShouldReturnMessagesByStatus_AndSetThemAsProcessing(OutboxMessage message)
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Act
        await uow.BeginTransactionAsync();
        await repo.AddAsync(message, uow.Connection, uow.Transaction!);
        await uow.CommitAsync();

        await uow.BeginTransactionAsync();
        var fetchedMessages = await repo.FetchMessagesByStatusAsync(1, MessageStatus.Pending, uow.Connection, uow.Transaction!);
        await uow.CommitAsync();

        // Assert
        var outboxMessage = fetchedMessages.ToList().FirstOrDefault();

        outboxMessage.Should().NotBeNull();
        outboxMessage!.Status.Should().Be(MessageStatus.Processing);
    }
}