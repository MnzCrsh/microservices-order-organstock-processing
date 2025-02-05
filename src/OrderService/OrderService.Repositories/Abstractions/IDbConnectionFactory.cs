using System.Data;
using Npgsql;

namespace OrderService.Repositories.Abstractions;

/// <summary>
/// Provides simpler access to sql connection
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates new <see cref="NpgsqlConnection"/>
    /// </summary>
    public IDbConnection CreateConnection();
}