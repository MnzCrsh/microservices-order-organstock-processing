using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Repositories.Helpers;

namespace OrderService.Fixture;

internal class FixtureFactoryBase(string connectionString) : WebApplicationFactory<Program>
{
    public IServiceScope CreateScopeInternal() =>
        base.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        base.ConfigureWebHost(builder);
        builder.ConfigureServices(services =>
        {
            RemoveApplicationServices(services);

            var dbConfig = new DbConfig { ConnectionString = connectionString };
            services.AddSingleton(dbConfig);
        });
    }

    private static void RemoveApplicationServices(IServiceCollection services)
    {
        var descriptors = services.Where(d =>
                d.ServiceType.Namespace != null &&
                d.ServiceType.Namespace.StartsWith("OrderService"))
            .ToList();

        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }
    }
}