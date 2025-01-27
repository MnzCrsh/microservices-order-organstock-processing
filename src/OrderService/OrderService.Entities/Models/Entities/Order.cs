namespace OrderService.Entities.Models.Entities;

/// <summary>
/// Record that represents order entity 
/// </summary>
public record Order
{
    /// <summary>
    /// Prefix number.
    /// Each service have unique prefix to avoid collisions.
    /// </summary>
    public byte IdPrefix { get; set; }
    
    public long IdInfix { get; set; }
    
    public byte IdPostfix { get; set; }
    
    public Guid CustomerId { get; init; }
    
    public int[] Items { get; init; } = null!;

    public decimal TotalAmount { get; init; }
    
    public OrderStatus OrderStatus { get; init; }

    public DateTimeOffset CreatedTime { get; init; }
    
    public DateTimeOffset UpdatedTime { get; init; }
}