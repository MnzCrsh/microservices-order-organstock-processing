using System.Data;
using OrderService.Entities.Models.Entities;
using OrderService.Entities.Models.Responses;

namespace OrderService.Repositories.Abstractions;

public interface IOrderRepository
{
    public Task<Order> AddAsync(Order order, IDbConnection connection, IDbTransaction transaction);

    public Task<bool> UpdateAsync(Order order, IDbConnection connection, IDbTransaction transaction);

    public Task<Order> GetByIdAsync(Guid orderId, IDbConnection connection, IDbTransaction transaction);

    public Task<IEnumerable<Guid>> GetTopThreeItems(IDbConnection connection, IDbTransaction transaction);
}