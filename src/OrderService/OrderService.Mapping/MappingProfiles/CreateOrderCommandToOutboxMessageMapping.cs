using System.Text.Json;
using OrderService.Entities;
using OrderService.Entities.Models.Commands;
using OrderService.Entities.Models.Entities;

namespace OrderService.Mapping.MappingProfiles;

public class CreateOrderCommandToOutboxMessageMapping : IMapper<CreateOrderCommand, OutboxMessage>
{
    public OutboxMessage Map(CreateOrderCommand source)
        => new()
        {
            Id = Guid.CreateVersion7(),
            EventType = nameof(CreateOrderCommand),
            Payload = JsonSerializer.Serialize(source),
            CreatedTime = DateTime.Now,
            Status = MessageStatus.Pending,
            RetryCount = 0
        };
}