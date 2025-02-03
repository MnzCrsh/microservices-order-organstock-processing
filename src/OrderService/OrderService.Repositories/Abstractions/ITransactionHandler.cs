using System.Data;

namespace OrderService.Repositories.Abstractions;

/// <summary>
/// Wraps repository actions in transaction
/// </summary>
public interface ITransactionHandler
{
    /// <summary>
    /// Starts transaction 
    /// </summary>
    /// <param name="action">Passed repository delegate</param>
    public Task ExecuteAsync(Func<IDbConnection, IDbTransaction, Task> action);

    /// <summary>
    /// Starts transaction 
    /// </summary>
    /// <param name="action">Passed repository delegate</param>
    /// <typeparam name="T">Transaction result</typeparam>
    public Task<T> ExecuteAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> action);

    /// <summary>
    /// Starts transaction 
    /// </summary>
    /// <param name="action">Passed repository delegate</param>
    /// <typeparam name="T1">Transaction result 1</typeparam>
    /// <typeparam name="T2">Transaction result 2</typeparam>
    public Task<(T1, T2)> ExecuteAsync<T1, T2>(Func<IDbConnection, IDbTransaction, Task<(T1, T2)>> action);

    /// <summary>
    /// Starts transaction 
    /// </summary>
    /// <param name="action">Passed repository delegate</param>
    /// <typeparam name="T1">Transaction result 1</typeparam>
    /// <typeparam name="T2">Transaction result 2</typeparam>
    /// <typeparam name="T3">Transaction result 3</typeparam>
    public Task<(T1, T2, T3)> ExecuteAsync<T1, T2, T3>(Func<IDbConnection, IDbTransaction, Task<(T1, T2, T3)>> action);
}