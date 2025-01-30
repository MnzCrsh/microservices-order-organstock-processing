using System.Text.Json;
using OrderService.Entities.Models.Entities;
using OrderService.Entities.Models.Responses;

namespace OrderService.Mapping.MappingProfiles;

public class OrderToResponseMapping : IMapper<Order, OrderResponseItem>
{
    public OrderResponseItem Map(Order source)
        => new()
        {
            OrderId = source.Id,
            CustomerId = source.CustomerId,
            Items = JsonSerializer.Deserialize<Guid[]>(source.Items)
                    ?? throw new InvalidOperationException("Unable to fetch order items"),
            TotalAmount = source.TotalAmount
        };
}