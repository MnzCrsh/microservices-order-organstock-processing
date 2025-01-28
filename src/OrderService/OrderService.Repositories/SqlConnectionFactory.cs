using System.Data;
using Microsoft.Data.SqlClient;

namespace OrderService.Repositories;

public class SqlConnectionFactory(DbConfig config) : IDbConnectionFactory
{
    public IDbConnection CreateConnection() => new SqlConnection(config.ConnectionString);
}