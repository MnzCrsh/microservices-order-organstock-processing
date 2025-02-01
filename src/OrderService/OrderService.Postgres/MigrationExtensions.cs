using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OrderService.Postgres.Migrations;

namespace OrderService.Postgres;

public static class MigrationExtensions
{
    /// <summary>
    /// Adds module with postgres migrations
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="connectionString">Connection string</param>
    public static IServiceCollection AddMigrations(this IServiceCollection services, string? connectionString)
    {
        if (connectionString.IsNullOrEmpty())
        {
            throw new ArgumentNullException($"{connectionString} cannot be null or empty.");
        }

        services
            .AddFluentMigratorCore()
            .ConfigureRunner(builder =>
        {
            builder
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(AddOrderAndOutboxTable).Assembly).For.Migrations();
        })
            .AddLogging(builder => builder.AddFluentMigratorConsole());

        return services;
    }

    /// <summary>
    /// Starts FluentMigrator migrations
    /// </summary>
    /// <param name="serviceProvider">Service collection</param>
    public static IServiceProvider RunMigrations(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        runner.MigrateUp();

        return scope.ServiceProvider;
    }
}