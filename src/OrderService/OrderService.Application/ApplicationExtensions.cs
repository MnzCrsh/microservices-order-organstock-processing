using Microsoft.Extensions.DependencyInjection;

namespace OrderService.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServicesModule(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();

        return services;
    }
}