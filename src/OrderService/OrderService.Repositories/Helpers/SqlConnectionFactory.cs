using System.Data;
using Microsoft.Data.SqlClient;
using OrderService.Repositories.Abstractions;

namespace OrderService.Repositories.Helpers;

public class SqlConnectionFactory(DbConfig config) : IDbConnectionFactory
{
    public IDbConnection CreateConnection() => new SqlConnection(config.ConnectionString);
}