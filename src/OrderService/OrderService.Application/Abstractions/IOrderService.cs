using OrderService.Entities.Models.Commands;
using OrderService.Entities.Models.Responses;

namespace OrderService.Application.Abstractions;

/// <summary>
/// Order service
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Asynchronously creates order 
    /// </summary>
    /// <param name="command">Create order command</param>
    public Task<OrderResponseItem> CreateAsync(CreateOrderCommand command);

    /// <summary>
    /// Asynchronously updates order
    /// </summary>
    /// <param name="command">Update order command</param>
    public Task<bool> UpdateAsync(UpdateOrderCommand command);

    /// <summary>
    /// Asynchronously fetches order by id
    /// </summary>
    /// <param name="id">Order id</param>
    public Task<OrderResponseItem> GetByIdAsync(Guid id);

    /// <summary>
    /// Asynchronously fetches top three requested items
    /// </summary>
    public Task<Guid[]> GetTopThreeAsync();
}