using OrderService.Entities.Models.Commands;
using OrderService.Entities.Models.Responses;
using OrderService.Validation;

namespace OrderService.CQRS.Abstractions;

public interface IOrderCommandProcessor
{
    public Task<(OrderResponseItem?, List<ValidationError>?)> ExecuteCreateAsync(CreateOrderCommand command);
    
    public Task<(OrderResponseItem?, List<ValidationError>?)> ExecuteUpdateAsync(CreateOrderCommand command);
}