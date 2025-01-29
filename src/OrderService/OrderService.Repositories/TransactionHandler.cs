using System.Data;
using OrderService.Repositories.Abstractions;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace OrderService.Repositories;

public class TransactionHandler(IDbConnectionFactory connectionFactory)
{
    public async Task ExecuteAsync(Func<IDbConnection, IDbTransaction, Task> action)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();
        
        using var transaction = connection.BeginTransaction((System.Data.IsolationLevel)IsolationLevel.ReadCommitted);
        
        await action(connection, transaction);
        
        transaction.Commit();
    }
}