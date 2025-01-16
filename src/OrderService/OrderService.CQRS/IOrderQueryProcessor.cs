using OrderService.Entities.Models.Responses;

namespace OrderService.CQRS;

public interface IOrderQueryProcessor
{
    Task<OrderResponseItem> GetOrderById(Guid id);
}