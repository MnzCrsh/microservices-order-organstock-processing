using OrderService.Entities;
using OrderService.Entities.Models.Commands;

namespace OrderService.Validation.ValidationProfiles;

public class CreateOrderCommandValidator : ValidatorBase<CreateOrderCommand>
{
    protected override ValueTask ValidateEntity(CreateOrderCommand entity, IList<ValidationError> errors)
    {
        if (entity.CustomerId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(entity.CustomerId), "Customer Id cannot be empty."));
        }

        if (entity.Items.Length is <= 0 or > OrderConstants.MaxItemsPerOrder)
        {
            errors.Add(new ValidationError(nameof(entity.Items), $"Items must be between 1 and {OrderConstants.MaxItemsPerOrder}"));
        }

        if (entity.TotalAmount is < OrderConstants.MinTotalAmount or > OrderConstants.MaxTotalAmount)
        {
            errors.Add(new ValidationError(nameof(entity.TotalAmount),
                $"Items must be between {OrderConstants.MinTotalAmount} and {OrderConstants.MaxTotalAmount}"));

        }

        return ValueTask.CompletedTask;
    }
}