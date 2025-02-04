using Microsoft.Extensions.DependencyInjection;

namespace OrderService.gRPC;

public static class GrpcExtensions
{
    public static IServiceCollection AddGrpcModule(this IServiceCollection services, bool isDevelopment)
    {
        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = isDevelopment;
            options.Interceptors.Add<GrpcExceptionInterceptor>();
        });

        services.AddScoped<OrderGrpcService>();

        return services;
    }
}