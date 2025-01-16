using OrderService.Entities.Models.Commands;

namespace OrderService.CQRS;

public interface IOrderCommandProcessor
{
    public Task<Guid> ExecuteCreateOrderCommandAsync(CreateOrderCommand command);
}