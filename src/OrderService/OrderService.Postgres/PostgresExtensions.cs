using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Postgres.Migrations;

namespace OrderService.Postgres;

public static class PostgresExtensions
{
    /// <summary>
    /// Adds module with postgres migrations
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="connectionString">Connection string</param>
    public static IServiceCollection AddPostgresMigrations(this IServiceCollection services, string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return services;
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

        services.Configure<SelectingProcessorAccessorOptions>(options =>
        {
            options.ProcessorId = "Postgres";
        });


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