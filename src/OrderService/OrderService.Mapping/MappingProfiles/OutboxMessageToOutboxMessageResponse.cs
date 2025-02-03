using OrderService.Entities.Models.Entities;
using OrderService.Entities.Models.Responses;

namespace OrderService.Mapping.MappingProfiles;

public class OutboxMessageToOutboxMessageResponse : IMapper<OutboxMessage, OutboxResponseModel>
{
    public OutboxResponseModel Map(OutboxMessage source)
        => new() { Id = source.Id, EventType = source.EventType, Payload = source.Payload };
}