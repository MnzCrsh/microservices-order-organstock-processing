using System.Data;
using OrderService.Entities.Models.Entities;
using OrderService.Entities.Models.Responses;

namespace OrderService.Repositories.Abstractions;

public interface IOrderRepository
{
    public Task<OrderResponseItem> AddAsync(Order order, IDbConnection connection, IDbTransaction transaction);
    
    public Task<bool> UpdateAsync(Order order, IDbConnection connection, IDbTransaction transaction);
    
    public Task<OrderResponseItem> GetByIdAsync(Guid orderId, IDbConnection connection, IDbTransaction transaction);
}