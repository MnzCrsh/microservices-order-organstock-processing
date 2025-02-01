using System.Data;
using Microsoft.Data.SqlClient;
using OrderService.Repositories.Abstractions;

namespace OrderService.Repositories.Helpers;

/// <inheritdoc/>
public class SqlConnectionFactory(DbConfig config) : IDbConnectionFactory
{
    /// <inheritdoc/>
    public IDbConnection CreateConnection() => new SqlConnection(config.ConnectionString);
}