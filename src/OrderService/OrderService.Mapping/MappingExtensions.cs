using Microsoft.Extensions.DependencyInjection;
using OrderService.Entities.Models.Commands;
using OrderService.Entities.Models.Entities;
using OrderService.Entities.Models.Responses;
using OrderService.Mapping.MappingProfiles;

namespace OrderService.Mapping;

public static class MappingExtensions
{
    public static IServiceCollection AddMappingModule(this IServiceCollection services)
    {
        services
            .AddScoped<IMapper<CreateOrderCommand, Order>, CommandToOrderMapping>()
            .AddScoped<IMapper<CreateOrderCommand, OutboxMessage>, OrderCommandToOutboxMessageMapping>()
            .AddScoped<IMapper<Order, OrderResponseItem>, OrderToResponseMapping>();

        return services;
    }
}