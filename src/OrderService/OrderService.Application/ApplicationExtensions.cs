using Microsoft.Extensions.DependencyInjection;

namespace OrderService.Application;

public static class ApplicationExtensions
{
    /// <summary>
    /// Adds module with domain services
    /// </summary>
    /// <param name="services">Service collection</param>
    public static IServiceCollection AddApplicationServicesModule(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();

        return services;
    }
}