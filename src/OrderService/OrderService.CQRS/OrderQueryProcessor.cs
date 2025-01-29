using System.Text.Json;
using OrderService.Entities.Models.Responses;
using StackExchange.Redis;

namespace OrderService.CQRS;

public class OrderQueryProcessor(IConnectionMultiplexer connectionMultiplexer, RedisConfig redisConfig) : IOrderQueryProcessor
{
    public async Task<OrderResponseItem> GetById(Guid id)
    {
        var redisDb = connectionMultiplexer.GetDatabase();
        var cacheKey = $"{redisConfig.OrderCacheKeyPrefix}:{id}";

        var cachedOrder = await redisDb.StringGetAsync(cacheKey);

        if (!cachedOrder.IsNullOrEmpty)
        {
            return JsonSerializer.Deserialize<OrderResponseItem>(cachedOrder!) ??
                   throw new InvalidOperationException("Cannot deserialize order response.");
        }

        // TODO: Receiving and Caching logic. Right now microservice with PostgreSQl as permanent storage is yet to be implemented.
        throw new NotImplementedException();
    }
}