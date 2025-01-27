namespace OrderService.Entities.Models.Events;

public record OrderCreatedEvent
{
    public long OrderId { get; init; }
    
    public Guid CustomerId { get; init; }
    
    public int[] Items { get; init; } = null!;
    
    public decimal TotalAmount { get; init; }
    
    public DateTimeOffset OrderDate { get; init; }
    
    public OrderStatus OrderStatus { get; init; }
}