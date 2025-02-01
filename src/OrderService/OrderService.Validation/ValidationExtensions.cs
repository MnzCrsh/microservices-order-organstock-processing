using Microsoft.Extensions.DependencyInjection;
using OrderService.Entities.Models.Commands;
using OrderService.Validation.ValidationProfiles;

namespace OrderService.Validation;

public static class ValidationExtensions
{
    /// <summary>
    /// Adds validation module
    /// </summary>
    /// <param name="services">Service collection</param>
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services
            .AddScoped<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>()
            .AddScoped<IValidator<UpdateOrderCommand>, UpdateOrderCommandValidator>();

        return services;
    }
}