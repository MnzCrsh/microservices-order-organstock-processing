namespace OrderService.Validation;

public interface IValidator<in T>
    where T : class
{
    public bool IsValid(T entity);

    public ValidationResult Validate(T entity);
}