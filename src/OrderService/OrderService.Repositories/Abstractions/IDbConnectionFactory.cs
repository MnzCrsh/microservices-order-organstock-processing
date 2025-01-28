using System.Data;

namespace OrderService.Repositories;

public interface IDbConnectionFactory
{
    public IDbConnection CreateConnection();
}