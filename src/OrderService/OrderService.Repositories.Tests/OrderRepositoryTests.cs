using System.Text.Json;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Entities;
using OrderService.Entities.Models.Entities;
using OrderService.Fixture;
using OrderService.Repositories.Abstractions;

namespace OrderService.Repositories.Tests;

public class OrderRepositoryTests(RepositoriesFixtureFactory factory, TestContainersFixture _) :
    IClassFixture<RepositoriesFixtureFactory>, IClassFixture<TestContainersFixture>
{

    [Theory(DisplayName = "AddAsync should insert new order into database"), AutoData]
    public async Task AddAsync_ShouldAddOrderToDb(Order order)
    {
        // Arrange
        using var scope = factory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        order = order with { Items = JsonSerializer.Serialize(order.Items) };

        // Act
        await uow.BeginTransactionAsync();

        var res = await repo.AddAsync(order, uow.Connection, uow.Transaction!);

        await uow.CommitAsync();

        // Assert
        res.Should().NotBeNull();
    }

    [Theory(DisplayName = "UpdateAsync should affect row in database"), AutoData]
    public async Task UpdateAsync_ShouldUpdateOrderInDb(Order order)
    {
        // Arrange
        using var scope = factory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        order = order with { Items = JsonSerializer.Serialize(order.Items) };

        // Act
        await uow.BeginTransactionAsync();
        var createRes = await repo.AddAsync(order, uow.Connection, uow.Transaction!);
        await uow.CommitAsync();

        var orderToUpdate = order with { Id = createRes.Id, OrderStatus = OrderStatus.PaymentRejected };

        await uow.BeginTransactionAsync();
        var isUpdated = await repo.UpdateAsync(orderToUpdate, uow.Connection, uow.Transaction!);
        await uow.CommitAsync();

        // Assert
        isUpdated.Should().BeTrue();
    }

    [Theory(DisplayName = "GetByIdAsync should fetch row from database"), AutoData]
    public async Task GetByIdAsync_ShouldReturnOrderFromDb(Order order)
    {
        // Arrange
        using var scope = factory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        order = order with { Items = JsonSerializer.Serialize(order.Items) };

        // Act
        await uow.BeginTransactionAsync();
        var createRes = await repo.AddAsync(order, uow.Connection, uow.Transaction!);
        await uow.CommitAsync();

        await uow.OpenConnectionAsync();
        var getRes = await repo.GetByIdAsync(createRes.Id, uow.Connection);

        // Assert
        getRes.Should().NotBeNull();
        getRes.Id.Should().Be(createRes.Id);
    }

    [Theory(DisplayName = "GetTopThreeItems should fetch top three items from database"), AutoData]
    public async Task GetTopThreeItems_ShouldReturnItemIds(Order order)
    {
        // Arrange
        using var scope = factory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        Guid[] items = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
        order = order with { Items = JsonSerializer.Serialize(items) };

        // Act
        await uow.BeginTransactionAsync();
        await repo.AddAsync(order, uow.Connection, uow.Transaction!);
        await uow.CommitAsync();

        await uow.OpenConnectionAsync();
        var getRes = await repo.GetTopThreeItems(uow.Connection);

        // Assert
        getRes.Should().NotBeNull().And.HaveCount(3);
    }
}