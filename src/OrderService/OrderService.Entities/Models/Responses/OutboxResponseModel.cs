namespace OrderService.Entities.Models.Responses;

public record OutboxResponseModel
{
    public Guid Id { get; init; }
    
    public string EventType {get; init; } = null!;
    
    public string Payload { get; init; } = null!;
    
    public DateTime CreatedTime { get; init; }
    
    public DateTime ProcessedTime { get; init; }
    
    public MessageStatus Status { get; init; }
    
    public int RetryCount { get; init; }
}