using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.CQRS.Abstractions;
using StackExchange.Redis;

namespace OrderService.CQRS;

public static class CqrsExtensions
{
    /// <summary>
    /// Adds CQRS module
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    public static IServiceCollection AddCqrsModule(this IServiceCollection services, IConfiguration configuration)
    {
        var redisSection = configuration.GetSection("Redis");

        var redisConfig = new RedisConfig();
        redisSection.Bind(redisConfig);
        services.AddSingleton(redisConfig);

        services.AddScoped<IOrderCommandProcessor, OrderCommandProcessor>();

        if (!string.IsNullOrEmpty(redisConfig.ConnectionString))
        {
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig.ConnectionString));
        }

        return services;
    }
}