using System.Data;
using OrderService.Repositories.Abstractions;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace OrderService.Repositories.Helpers;

/// <summary>
/// Used for wrapping repositories inside transaction and passing it inside.
/// </summary>
/// <param name="connectionFactory">Factory for instantiating connection</param>
public class TransactionHandler(IDbConnectionFactory connectionFactory)
{
    /// <summary>
    /// Starts transaction 
    /// </summary>
    /// <param name="action">Passed repository delegate</param>
    /// <typeparam name="T">Transaction result</typeparam>
    public async Task<T> ExecuteAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> action)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction((System.Data.IsolationLevel)IsolationLevel.ReadCommitted);

        try
        {
            var result = await action(connection, transaction);

            transaction.Commit();

            return result;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new ApplicationException("Transaction failed.", ex);
        }
    }

    /// <summary>
    /// Starts transaction 
    /// </summary>
    /// <param name="action">Passed repository delegate</param>
    /// <typeparam name="T1">Transaction result 1</typeparam>
    /// <typeparam name="T2">Transaction result 2</typeparam>
    public async Task<(T1, T2)> ExecuteAsync<T1, T2>(Func<IDbConnection, IDbTransaction, Task<(T1, T2)>> action)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction((System.Data.IsolationLevel)IsolationLevel.ReadCommitted);

        try
        {
            var result = await action(connection, transaction);

            transaction.Commit();

            return result;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new ApplicationException("Transaction failed.", ex);
        }
    }

    /// <summary>
    /// Starts transaction 
    /// </summary>
    /// <param name="action">Passed repository delegate</param>
    /// <typeparam name="T1">Transaction result 1</typeparam>
    /// <typeparam name="T2">Transaction result 2</typeparam>
    /// <typeparam name="T3">Transaction result 3</typeparam>
    public async Task<(T1, T2, T3)> ExecuteAsync<T1, T2, T3>(Func<IDbConnection, IDbTransaction, Task<(T1, T2, T3)>> action)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction((System.Data.IsolationLevel)IsolationLevel.ReadCommitted);

        try
        {
            var result = await action(connection, transaction);

            transaction.Commit();

            return result;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new ApplicationException("Transaction failed.", ex);
        }
    }
}