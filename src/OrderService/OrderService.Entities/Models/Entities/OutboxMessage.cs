namespace OrderService.Entities.Models.Entities;

public record OutboxMessage
{
    public Guid Id { get; init; }

    public string EventType { get; init; } = null!;

    public string Payload { get; init; } = null!;

    public DateTimeOffset CreatedTime { get; init; }

    public DateTimeOffset? ProcessedTime { get; init; }

    public MessageStatus Status { get; init; }

    public int RetryCount { get; init; }
}