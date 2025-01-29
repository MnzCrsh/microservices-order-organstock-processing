using System.Data;

namespace OrderService.Repositories.Abstractions;

public interface IDbConnectionFactory
{
    public IDbConnection CreateConnection();
}