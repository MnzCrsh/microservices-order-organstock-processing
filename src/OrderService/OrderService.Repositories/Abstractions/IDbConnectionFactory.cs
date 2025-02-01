using System.Data;
using Microsoft.Data.SqlClient;

namespace OrderService.Repositories.Abstractions;

/// <summary>
/// Provides simpler access to sql connection
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates new <see cref="SqlConnection"/>
    /// </summary>
    public IDbConnection CreateConnection();
}