using System.Data;
using Microsoft.Extensions.Logging;
using Npgsql;
using OrderService.Repositories.Abstractions;
using OrderService.Repositories.Helpers;

namespace OrderService.Repositories;

/// <inheritdoc cref="OrderService.Repositories.Abstractions.IUnitOfWork" />
/// <remarks>Concrete implementation with Npgsql driver.</remarks>
public class NpgsqlUnitOfWork(DbConfig dbConfig, ILogger<NpgsqlUnitOfWork> logger) : IUnitOfWork, IDisposable, IAsyncDisposable
{
    /// <inheritdoc/>
    public IDbConnection Connection => _connection ?? throw new InvalidOperationException("Connection not initialized");

    /// <inheritdoc/>
    public IDbTransaction? Transaction => _transaction;

    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;

    /// <inheritdoc/>
    public async Task OpenConnectionAsync()
    {
        if (_connection != null)
        {
            logger.LogWarning("The connection {Connection} is already open.", _connection);
            return;
        }

        _connection = new NpgsqlConnection(dbConfig.ConnectionString);
        await _connection.OpenAsync();
    }

    /// <inheritdoc/>
    public async Task BeginTransactionAsync()
    {
        if (_connection != null)
        {
            throw new InvalidOperationException("Transaction already started");
        }

        _connection = new NpgsqlConnection(dbConfig.ConnectionString);

        logger.LogInformation("Opening connection");
        await _connection.OpenAsync();

        logger.LogInformation("Beginning transaction");
        _transaction = await _connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);
    }

    /// <inheritdoc/>
    public async Task CommitAsync()
    {
        if (_transaction == null)
        {
            logger.LogError("Commit attempted with no active transaction");
            throw new InvalidOperationException("No active transaction to commit");
        }

        logger.LogInformation("Committing transaction");
        await _transaction.CommitAsync();
        await DisposeAsync();
    }

    /// <inheritdoc/>
    public async Task RollbackAsync()
    {
        if (_transaction == null)
        {
            logger.LogWarning("Rollback attempted with no active transaction");
            return;
        }

        logger.LogInformation("Rolling back transaction");
        await _transaction.RollbackAsync();
        await DisposeAsync();
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        if (_connection != null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_transaction != null)
        {
            _transaction.Dispose();
            _transaction = null;
        }

        if (_connection != null)
        {
            _connection.Dispose();
            _connection = null;
        }
    }
}