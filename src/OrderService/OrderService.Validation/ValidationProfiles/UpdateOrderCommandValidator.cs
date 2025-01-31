using OrderService.Application;
using OrderService.Entities.Models.Commands;

namespace OrderService.Validation.ValidationProfiles;

public class UpdateOrderCommandValidator(IOrderService orderService) : ValidatorBase<UpdateOrderCommand>
{
    protected override async Task ValidateEntity(UpdateOrderCommand entity, IList<ValidationError> errors)
    {
        var entityFromDb = await orderService.GetByIdAsync(entity.Id);
        if (entity.OrderStatus < entityFromDb.Status)
        {
            errors.Add(new ValidationError(nameof(entity.OrderStatus), "Invalid status transition"));
        }
    }
}