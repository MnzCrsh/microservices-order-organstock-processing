namespace OrderService.Entities.Models.Entities;

/// <summary>
/// Record that represents order database entity 
/// </summary>
public record Order
{
    public Guid Id { get; init; }

    public Guid CustomerId { get; init; }

    public string Items { get; init; } = null!;

    public decimal TotalAmount { get; init; }

    public OrderStatus OrderStatus { get; init; }

    public DateTimeOffset CreatedTime { get; init; }

    public DateTimeOffset UpdatedTime { get; init; }
}