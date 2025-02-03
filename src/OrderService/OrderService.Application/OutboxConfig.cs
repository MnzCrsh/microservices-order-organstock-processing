namespace OrderService.Application;

public class OutboxConfig
{
    public int BatchSize { get; init; }

    public int DelayInMilliseconds { get; init; }
}