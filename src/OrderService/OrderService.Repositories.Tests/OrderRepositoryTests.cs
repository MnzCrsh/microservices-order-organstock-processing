using System.Text.Json;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
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

        // Assert
        res.Should().NotBeNull();
    }
}