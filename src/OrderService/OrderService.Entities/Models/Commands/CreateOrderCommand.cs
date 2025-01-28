namespace OrderService.Entities.Models.Commands;

public record CreateOrderCommand
{
    public Guid CustomerId { get; init; }
    
    public Guid[] Items { get; init; } = null!;

    public decimal TotalAmount { get; init; }
}