namespace OrderService.Entities.Models.Events;

public record OrderCreatedEvent
{
    public Guid OrderId { get; init; }
    
    public Guid CustomerId { get; init; }
    
    public DateTimeOffset OrderDate { get; init; }
    
    public decimal TotalAmount { get; init; }

    public Guid[] Items { get; init; } = null!;
    
    public OrderStatus OrderStatus { get; init; }
}