namespace OrderService.Entities.Models.Responses;

public record OutboxResponseModel
{
    public Guid Id { get; init; }

    public string EventType { get; init; } = null!;

    public string Payload { get; init; } = null!;
}