namespace OrderService.Validation;

/// <summary>
/// Request validator
/// </summary>
/// <typeparam name="T">Request to validate</typeparam>
public interface IValidator<in T>
    where T : class
{
    /// <summary>
    /// Request model is valid
    /// </summary>
    /// <param name="entity">Request</param>
    public bool IsValid(T entity);

    /// <summary>
    /// Validates request by defined rules
    /// </summary>
    /// <param name="entity">Request</param>
    public ValidationResult Validate(T entity);
}