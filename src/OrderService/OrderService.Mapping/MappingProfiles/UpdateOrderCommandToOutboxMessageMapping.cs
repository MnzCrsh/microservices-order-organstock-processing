using System.Text.Json;
using OrderService.Entities;
using OrderService.Entities.Models.Commands;
using OrderService.Entities.Models.Entities;

namespace OrderService.Mapping.MappingProfiles;

public class UpdateOrderCommandToOutboxMessageMapping : IMapper<UpdateOrderCommand, OutboxMessage>
{
    public OutboxMessage Map(UpdateOrderCommand source) 
        => new()
    {
        Id = Guid.CreateVersion7(),
        EventType = nameof(UpdateOrderCommand),
        Payload = JsonSerializer.Serialize(source),
        Status = MessageStatus.Pending,
    };
}