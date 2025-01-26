using OrderService.Entities.Models.Commands;
using OrderService.Entities.Models.Responses;

namespace OrderService.Repositories;

public interface IOrderRepository
{
    public Task<OrderResponseItem> Save(CreateOrderCommand command);
}