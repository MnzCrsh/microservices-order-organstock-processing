using System.Data;
using Npgsql;
using OrderService.Repositories.Abstractions;

namespace OrderService.Repositories.Helpers;

/// <inheritdoc/>
public class SqlConnectionFactory(DbConfig config) : IDbConnectionFactory
{
    /// <inheritdoc/>
    public IDbConnection CreateConnection() => new NpgsqlConnection(config.ConnectionString);
}