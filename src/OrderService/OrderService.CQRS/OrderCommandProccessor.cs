using OrderService.Application;
using OrderService.CQRS.Abstractions;
using OrderService.Entities.Models.Commands;
using OrderService.Entities.Models.Responses;

namespace OrderService.CQRS;

public class OrderCommandProcessor(IOrderService orderService) : IOrderCommandProcessor
{
    public async Task<OrderResponseItem> ExecuteCreateAsync(CreateOrderCommand command)
    {
        return await orderService.CreateAsync(command);
    }
}