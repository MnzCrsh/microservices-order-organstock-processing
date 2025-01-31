using Microsoft.Extensions.DependencyInjection;

namespace OrderService.Validation;

public static class ValidationExtensions
{
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        return services;
    }
}