using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Abstractions;

namespace OrderService.Application;

public static class ApplicationExtensions
{
    /// <summary>
    /// Adds module with domain services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="outboxSection">IConfiguration</param>
    public static IServiceCollection AddApplicationServicesModule(this IServiceCollection services, IConfigurationSection outboxSection)
    {
        return services.
            AddScoped<IOrderService, OrderService>()
            .RegisterOutboxConfig(outboxSection);
    }

    private static IServiceCollection RegisterOutboxConfig(this IServiceCollection services, IConfigurationSection outboxSection)
    {
        var dbConfig = new OutboxConfig();
        outboxSection.Bind(dbConfig);
        services.AddSingleton(dbConfig);

        return services;
    }
}