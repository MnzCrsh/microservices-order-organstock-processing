namespace OrderService.CQRS;

public class RedisConfig
{
    public string ConnectionString { get; set; } = null!;
    public string OrderCacheKeyPrefix { get; set; } = null!;
}