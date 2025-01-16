using System.Text.Json;
using OrderService.Entities.Models.Responses;
using StackExchange.Redis;

namespace OrderService.CQRS;

public class OrderQueryProcessor(IConnectionMultiplexer connectionMultiplexer) : IOrderQueryProcessor
{
    public async Task<OrderResponseItem> GetOrderById(Guid id)
    {
        var redisDb = connectionMultiplexer.GetDatabase();
        var cacheKey = $"order:{id}";
        
        var cachedOrder = await redisDb.StringGetAsync(cacheKey);

        if (!cachedOrder.IsNullOrEmpty)
        {
            return JsonSerializer.Deserialize<OrderResponseItem>(cachedOrder!) ?? throw new InvalidOperationException();
        }
        
        // TODO: Receiving and Caching logic. Right now microservice with PostgreSQl as permanent storage is yet to be implemented.
        throw new NotImplementedException();
    }
}