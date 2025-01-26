namespace OrderService.Entities.Models.Commands;

public record CreateOrderCommand
{
    public long CustomerId { get; init; }
    
    public int[] Items { get; init; } = null!;

    public decimal TotalAmount { get; init; }
}