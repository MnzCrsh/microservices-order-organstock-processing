using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Repositories.Helpers;

namespace OrderService.Fixture;

internal class FixtureFactoryBase : WebApplicationFactory<Program>
{
    protected const string SqlConnectionString = "Data Source=:memory:;Version=3;New=True;";
    
    public IServiceScope CreateScopeInternal() => 
        base.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(services =>
        {
            var dbConfig = new DbConfig { ConnectionString = SqlConnectionString };
            services.AddSingleton(dbConfig);
        });
    }
}