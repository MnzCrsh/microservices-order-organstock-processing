namespace OrderService.Entities.Models.Commands;

public class UpdateOrderCommand
{
    public Guid Id { get; set; }
    
    public OrderStatus OrderStatus { get; init; }
    
    public DateTimeOffset UpdatedTime { get; init; }
}