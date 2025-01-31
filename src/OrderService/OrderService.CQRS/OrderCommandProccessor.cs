using Microsoft.Extensions.Logging;
using OrderService.Application;
using OrderService.CQRS.Abstractions;
using OrderService.Entities.Models.Commands;
using OrderService.Entities.Models.Responses;
using OrderService.Validation;

namespace OrderService.CQRS;

public class OrderCommandProcessor(IOrderService orderService,
    IValidator<CreateOrderCommand> creationValidator,
    IValidator<UpdateOrderCommand> updateValidator,
    ILogger<OrderCommandProcessor> logger) : IOrderCommandProcessor
{
    public async Task<(OrderResponseItem?, List<ValidationError>?)> ExecuteCreateAsync(CreateOrderCommand command)
    {
        logger.LogInformation("Execute Create order command");

        logger.LogInformation("Execute Create order command validation");

        var validationResult = creationValidator.Validate(command);

        if (validationResult.IsValid)
        {
            logger.LogInformation("Validation result is valid");
            return (await orderService.CreateAsync(command), null);
        }

        foreach (var error in validationResult.Errors)
        {
            logger.LogWarning("Validation Error {Error}", error.ErrorMessage);
        }

        return (null, validationResult.Errors);
    }

    public async Task<(bool, List<ValidationError>?)> ExecuteUpdateAsync(UpdateOrderCommand command)
    {
        logger.LogInformation("Execute Update order command");

        logger.LogInformation("Execute Update order command validation");

        var validationResult = updateValidator.Validate(command);

        if (validationResult.IsValid)
        {
            logger.LogInformation("Validation result is valid");
            return (await orderService.UpdateAsync(command), null);
        }

        foreach (var error in validationResult.Errors)
        {
            logger.LogWarning("Validation Error {Error}", error.ErrorMessage);
        }

        return (false, validationResult.Errors);
    }
}