using System.Data;
using OrderService.Entities.Models.Entities;

namespace OrderService.Repositories.Abstractions;

/// <summary>
/// Provides abstraction over sql commands executed upon <see cref="Order"/> table
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Asynchronously adds order to database
    /// </summary>
    /// <param name="order">Order entity</param>
    /// <param name="connection">Sql connection</param>
    /// <param name="transaction">Sql transaction</param>
    public Task<Order> AddAsync(Order order, IDbConnection connection, IDbTransaction transaction);

    /// <summary>
    /// Asynchronously updates order from database
    /// </summary>
    /// <param name="order">Order entity</param>
    /// <param name="connection">Sql connection</param>
    /// <param name="transaction">Sql transaction</param>
    public Task<bool> UpdateAsync(Order order, IDbConnection connection, IDbTransaction transaction);

    /// <summary>
    /// Asynchronously fetches order from database by id
    /// </summary>
    /// <param name="orderId">Order entity</param>
    /// <param name="connection">Sql connection</param>
    /// <param name="transaction">Sql transaction</param>
    public Task<Order> GetByIdAsync(Guid orderId, IDbConnection connection);

    /// <summary>
    /// Asynchronously fetches top three requested items from database
    /// </summary>
    /// <param name="connection">Sql connection</param>
    /// <param name="transaction">Sql transaction</param>
    public Task<IEnumerable<Guid>> GetTopThreeItems(IDbConnection connection);
}