using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Fixture;
using OrderService.Postgres;
using OrderService.Repositories.Helpers;

namespace OrderService.Repositories.Tests;

public class RepositoriesFixtureFactory(string connectionString)
{
    private readonly RepositoriesFixtureFactoryImpl _privateFactory = new(connectionString);

    public IServiceScope CreateScope() => _privateFactory.CreateScopeInternal();

    private class RepositoriesFixtureFactoryImpl(string connectionString) : FixtureFactoryBase(connectionString)
    {
        private readonly string _connectionString = connectionString;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureServices(services =>
            {
                services
                    .AddRepositoriesModule()
                    .AddPostgresMigrations(_connectionString);
            });
        }
    }
}