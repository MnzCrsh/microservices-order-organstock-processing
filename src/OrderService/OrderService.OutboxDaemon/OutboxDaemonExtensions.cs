using Microsoft.Extensions.DependencyInjection;

namespace OrderService.OutboxDaemon;

public static class OutboxDaemonExtensions
{
    public static IServiceCollection AddOutboxDaemon(this IServiceCollection services)
    {
        services.AddHostedService<OutboxDaemon>();
        return services;
    }
}