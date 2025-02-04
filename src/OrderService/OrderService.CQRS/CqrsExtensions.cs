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
    public static IServiceCollection AddCqrs(this IServiceCollection services, IConfiguration configuration)
    {
        var redisSection = configuration.GetSection("Redis");

        var redisConfig = new RedisConfig();
        redisSection.Bind(redisConfig);
        services.AddSingleton(redisConfig);

        services.AddScoped<IOrderCommandProcessor, OrderCommandProcessor>();
        
        // services
        //     .AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig.ConnectionString));

        return services;
    }
}