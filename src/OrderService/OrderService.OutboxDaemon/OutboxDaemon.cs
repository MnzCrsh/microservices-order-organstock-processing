using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderService.Application;
using OrderService.Application.Abstractions;

namespace OrderService.OutboxDaemon;

public class OutboxDaemon(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested is false)
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IOutboxService>();
            var config = scope.ServiceProvider.GetRequiredService<OutboxConfig>();

            await service.ProcessAsync();

            await Task.Delay(config.DelayInMilliseconds, stoppingToken);
        }
    }
}