using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Fixture;
using OrderService.Postgres;
using OrderService.Repositories.Helpers;

namespace OrderService.Repositories.Tests;

public class RepositoriesFixtureFactory
{
    private readonly RepositoriesFixtureFactoryImpl _privateFactory = new();

    public IServiceScope CreateScope() => _privateFactory.CreateScopeInternal();
    
    private class RepositoriesFixtureFactoryImpl : FixtureFactoryBase
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
        
            builder.ConfigureServices(services =>
            {
                services
                    .AddRepositoriesModule()
                    .AddPostgresMigrations(TestContainersFixture.PostgresConnectionString);
            });
        }
    }
}