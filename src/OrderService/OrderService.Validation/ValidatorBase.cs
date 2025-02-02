namespace OrderService.Validation;

/// <inheritdoc/>
public abstract class ValidatorBase<T> : IValidator<T>
    where T : class
{
    /// <inheritdoc/>
    public virtual bool IsValid(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
        }

        var validationResult = Validate(entity);
        return validationResult.IsValid;
    }

    /// <inheritdoc/>
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

    /// <summary>
    /// Internal implementation of entity by derived class 
    /// </summary>
    /// <param name="entity">Request</param>
    /// <param name="errors">Aggregated errors</param>
    protected abstract ValueTask ValidateEntity(T entity, IList<ValidationError> errors);
}