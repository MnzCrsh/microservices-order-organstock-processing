using OrderService.Entities.Models.Commands;
using OrderService.Entities.Models.Responses;

namespace OrderService.Application;

public interface IOrderService
{
    public Task<OrderResponseItem> CreateAsync(CreateOrderCommand command);

    public Task<bool> UpdateAsync(UpdateOrderCommand command);

    public Task<OrderResponseItem> GetByIdAsync(Guid id);

    public Task<Guid[]> GetTopThreeAsync();
}