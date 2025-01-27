namespace OrderService.Entities.Models.Entities;

/// <summary>
/// Record that represents order database entity 
/// </summary>
public record Order
{
    
    /// <summary>
    /// Random byte as id infix.
    /// </summary>
    public byte IdInfix { get; init; }
    
    /// <summary>
    /// Prefix + Infix + Postfix.
    /// </summary>
    public long CombinedId { get; init; }
    
    public Guid CustomerId { get; init; }
    
    public int[] Items { get; init; } = null!;

    public decimal TotalAmount { get; init; }
    
    public OrderStatus OrderStatus { get; init; }

    public DateTimeOffset CreatedTime { get; init; }
    
    public DateTimeOffset UpdatedTime { get; init; }
}