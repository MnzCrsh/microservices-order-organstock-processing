using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Repositories.Abstractions;

namespace OrderService.Repositories.Helpers;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositoriesModule(this IServiceCollection services, IConfigurationSection sqlSection)
    {
        services
            .AddScoped<IOrderRepository, OrderRepository>()
            .AddScoped<IOutboxRepository, OutboxRepository>()
            .AddScoped<IDbConnectionFactory, SqlConnectionFactory>()
            .AddScoped<ITransactionHandler, TransactionHandler>();

        services.RegisterSqlConnection(sqlSection);

        return services;
    }

    private static IServiceCollection RegisterSqlConnection(this IServiceCollection services, IConfigurationSection sqlSection)
    {
        var dbConfig = new DbConfig();
        sqlSection.Bind(dbConfig);
        services.AddSingleton(dbConfig);

        return services;
    }
}