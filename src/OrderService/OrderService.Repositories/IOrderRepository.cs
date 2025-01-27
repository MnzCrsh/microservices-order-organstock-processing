using OrderService.Entities.Models.Entities;
using OrderService.Entities.Models.Responses;

namespace OrderService.Repositories;

public interface IOrderRepository
{
    public Task<OrderResponseItem> AddAsync(Order order);
    
    public Task<OrderResponseItem> GetByIdAsync(long orderId);
}