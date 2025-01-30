using System.Text.Json;
using OrderService.Entities;
using OrderService.Entities.Models.Commands;
using OrderService.Entities.Models.Entities;

namespace OrderService.Mapping.MappingProfiles;

public class CommandToOrderMapping : IMapper<CreateOrderCommand, Order>
{
    private static Order CreateOrderFromCommand(CreateOrderCommand command)
        => new()
        {
            Id = Guid.CreateVersion7(),
            CustomerId = command.CustomerId,
            Items = JsonSerializer.Serialize(command.Items),
            TotalAmount = command.TotalAmount,
            OrderStatus = OrderStatus.Created,
            CreatedTime = DateTimeOffset.UtcNow,
            UpdatedTime = DateTimeOffset.UtcNow
        };

    public Order Map(CreateOrderCommand source) => CreateOrderFromCommand(source);
}