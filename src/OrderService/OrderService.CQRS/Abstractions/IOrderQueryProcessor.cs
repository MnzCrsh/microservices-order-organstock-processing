using OrderService.Entities.Models.Responses;

namespace OrderService.CQRS.Abstractions;

public interface IOrderQueryProcessor
{
    Task<OrderResponseItem> GetById(Guid id);
}