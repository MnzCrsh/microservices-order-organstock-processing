using System.Data;
using Microsoft.Extensions.Logging;
using OrderService.Repositories.Abstractions;
using Npgsql;

namespace OrderService.Repositories.Helpers;

/// <summary>
/// Used for wrapping repositories inside transaction and passing it inside.
/// </summary>
/// <param name="connectionFactory">Factory for instantiating connection</param>
public class TransactionHandler(IDbConnectionFactory connectionFactory, ILogger<TransactionHandler> logger) : ITransactionHandler
{
    /// <inheritdoc/>
    public async Task ExecuteAsync(Func<IDbConnection, IDbTransaction, Task> action)
    {
        await using var connection = connectionFactory.CreateConnection() as NpgsqlConnection;

        if (connection is null)
        {
            logger.LogCritical("Failed to create connection");
            throw new ApplicationException("Connection is not Npgsql or not instantiated");
        }
        
        logger.LogInformation("Opening connection");
        await connection.OpenAsync();
        logger.LogInformation("Connection opened");
        
        logger.LogInformation("Beginning transaction");
        var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            logger.LogInformation("Executing action {Action}", action.Method.Name);
            await action(connection, transaction);
            logger.LogInformation("Action {Action} executed with success", action.Method.Name);

            logger.LogInformation("Committing transaction");
            await transaction.CommitAsync();
            logger.LogInformation("Transaction committed");
        }
        catch (Exception)
        {
            logger.LogError("Action {Action} failed. Beginning rollback.", action.Method.Name);
            await transaction.RollbackAsync();
            logger.LogError("Rollback executed.");

            throw;
        }
        finally
        {
            logger.LogInformation("Closing connection");
            await transaction.DisposeAsync();
            logger.LogInformation("Connection closed");
        }
    }

    /// <summary>
    /// Starts transaction 
    /// </summary>
    /// <param name="action">Passed repository delegate</param>
    /// <typeparam name="T">Transaction result</typeparam>
    public async Task<T> ExecuteAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> action)
    {
        await using var connection = connectionFactory.CreateConnection() as NpgsqlConnection;

        if (connection is null)
        {
            logger.LogCritical("Failed to create connection");
            throw new ApplicationException("Connection is not Npgsql or not instantiated");
        }
        
        logger.LogInformation("Opening connection");
        await connection.OpenAsync();
        logger.LogInformation("Connection opened");
        
        logger.LogInformation("Beginning transaction");
        var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            logger.LogInformation("Executing action {Action}", action.Method.Name);
            var result = await action(connection, transaction);
            logger.LogInformation("Action {Action} executed with success", action.Method.Name);

            logger.LogInformation("Committing transaction");
            await transaction.CommitAsync();
            logger.LogInformation("Transaction committed");
            
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError("Action {Action} failed. Beginning rollback.", action.Method.Name);
            await transaction.RollbackAsync();
            logger.LogError("Rollback executed.");

            throw new ApplicationException("Transaction failed.", ex);
        }
        finally
        {
            logger.LogInformation("Closing connection");
            await transaction.DisposeAsync();
            logger.LogInformation("Connection closed");
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
        await using var connection = connectionFactory.CreateConnection() as NpgsqlConnection;

        if (connection is null)
        {
            logger.LogCritical("Failed to create connection");
            throw new ApplicationException("Connection is not Npgsql or not instantiated");
        }
        
        logger.LogInformation("Opening connection");
        await connection.OpenAsync();
        logger.LogInformation("Connection opened");
        
        logger.LogInformation("Beginning transaction");
        var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            logger.LogInformation("Executing action {Action}", action.Method.Name);
            var result = await action(connection, transaction);
            logger.LogInformation("Action {Action} executed with success", action.Method.Name);

            logger.LogInformation("Committing transaction");
            await transaction.CommitAsync();
            logger.LogInformation("Transaction committed");
            
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError("Action {Action} failed. Beginning rollback.", action.Method.Name);
            await transaction.RollbackAsync();
            logger.LogError("Rollback executed.");

            throw new ApplicationException("Transaction failed.", ex);
        }
        finally
        {
            logger.LogInformation("Closing connection");
            await transaction.DisposeAsync();
            logger.LogInformation("Connection closed");
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
        await using var connection = connectionFactory.CreateConnection() as NpgsqlConnection;

        if (connection is null)
        {
            logger.LogCritical("Failed to create connection");
            throw new ApplicationException("Connection is not Npgsql or not instantiated");
        }
        
        logger.LogInformation("Opening connection");
        await connection.OpenAsync();
        logger.LogInformation("Connection opened");
        
        logger.LogInformation("Beginning transaction");
        var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            logger.LogInformation("Executing action {Action}", action.Method.Name);
            var result = await action(connection, transaction);
            logger.LogInformation("Action {Action} executed with success", action.Method.Name);

            logger.LogInformation("Committing transaction");
            await transaction.CommitAsync();
            logger.LogInformation("Transaction committed");
            
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError("Action {Action} failed. Beginning rollback.", action.Method.Name);
            await transaction.RollbackAsync();
            logger.LogError("Rollback executed.");

            throw new ApplicationException("Transaction failed.", ex);
        }
        finally
        {
            logger.LogInformation("Closing connection");
            await transaction.DisposeAsync();
            logger.LogInformation("Connection closed");
        }
    }
}