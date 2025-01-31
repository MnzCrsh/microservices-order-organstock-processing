using OrderService.Entities.Models.Commands;
using OrderService.Entities.Models.Entities;

namespace OrderService.Mapping.MappingProfiles;

public class UpdateOrderCommandToOrderMapping : IMapper<UpdateOrderCommand, Order>
{
    public Order Map(UpdateOrderCommand source) => new()
    {
        OrderStatus = source.OrderStatus,
        UpdatedTime = DateTimeOffset.Now
    };
}