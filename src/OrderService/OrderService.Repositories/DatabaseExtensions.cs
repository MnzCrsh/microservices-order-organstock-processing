using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OrderService.Repositories;

public static class DatabaseExtensions
{
    public static IServiceCollection AddSqlConnection(this IServiceCollection services, IConfigurationSection sqlSection)
    {
        var dbConfig = new DbConfig();
        sqlSection.Bind(dbConfig);
        services.AddSingleton(dbConfig);

        return services;
    }
}