using OrderService.Entities.Models.Commands;
using OrderService.Entities.Models.Responses;

namespace OrderService.CQRS.Abstractions;

public interface IOrderCommandProcessor
{
    public Task<OrderResponseItem> ExecuteCreateAsync(CreateOrderCommand command);
}