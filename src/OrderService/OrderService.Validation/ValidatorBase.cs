namespace OrderService.Validation;

public abstract class ValidatorBase<T> : IValidator<T>
    where T : class
{
    public virtual bool IsValid(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
        }

        var validationResult = Validate(entity);
        return validationResult.IsValid;
    }

    public virtual ValidationResult Validate(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
        }

        var errors = new List<ValidationError>();
        ValidateEntity(entity, errors);

        return new ValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };
    }

    protected abstract Task ValidateEntity(T entity, IList<ValidationError> errors);
}