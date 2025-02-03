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
            .AddScoped<IMapper<CreateOrderCommand, Order>, CreateOrderCommandToOrderMapping>()
            .AddScoped<IMapper<CreateOrderCommand, OutboxMessage>, CreateOrderCommandToOutboxMessageMapping>()
            .AddScoped<IMapper<Order, OrderResponseItem>, OrderToResponseMapping>()
            .AddScoped<IMapper<OutboxMessage, OutboxResponseModel>, OutboxMessageToOutboxMessageResponse>();

        services.AddScoped<IMapperFactory, MapperFactory>();

        return services;
    }
}