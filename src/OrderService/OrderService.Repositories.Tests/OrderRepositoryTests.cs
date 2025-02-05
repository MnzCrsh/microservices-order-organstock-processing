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
        var transactionHandler = scope.ServiceProvider.GetRequiredService<ITransactionHandler>();
        
        // Act
        var res = await transactionHandler.ExecuteAsync(async (connection, dbTransaction) =>
            await repo.AddAsync(order, connection, dbTransaction));
        
        // Assert
        res.Should().NotBeNull();
    }
}