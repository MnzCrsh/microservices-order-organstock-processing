namespace OrderService.Entities.Models.Responses;

public record OrderResponseItem
{
    public Guid OrderId { get; init; }

    public Guid CustomerId { get; init; }

    public Guid[] Items { get; init; } = null!;
    
    public OrderStatus Status { get; init; }

    public decimal TotalAmount { get; init; }
}