using OrderService.Entities.Models.Commands;
using OrderService.Entities.Models.Responses;

namespace OrderService.Repositories;

public class OrderRepository : IOrderRepository
{
    public Task<OrderResponseItem> Save(CreateOrderCommand command)
    {
        throw new NotImplementedException();
    }
}