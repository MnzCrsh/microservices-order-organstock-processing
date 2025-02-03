using Microsoft.Extensions.Hosting;
using OrderService.Application;
using OrderService.Application.Abstractions;

namespace OrderService.OutboxDaemon;

public class OutboxDaemon(IOutboxService service, OutboxConfig config) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested is false)
        {
            await service.ProcessAsync();

            await Task.Delay(config.DelayInMilliseconds, stoppingToken);
        }
    }
}