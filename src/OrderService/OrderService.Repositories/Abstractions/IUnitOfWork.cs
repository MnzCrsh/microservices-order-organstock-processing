using System.Data;

namespace OrderService.Repositories.Abstractions;

/// <summary>
/// Interface for the unit of work, that helps manage transactions and save changes to the database.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Opens connection to Sql database.
    /// <remarks>Used when operation should be executed out of transaction scope i.e. Fetching Data.</remarks>
    /// </summary>
    public Task OpenConnectionAsync();

    /// <summary>
    /// Opens connection to Sql database and starts transaction.
    /// <remarks>Do not require starting connection beforehand.</remarks>
    /// </summary>
    public Task BeginTransactionAsync();

    /// <summary>
    /// Commits current transaction. 
    /// </summary>
    public Task CommitAsync();

    /// <summary>
    /// Rollbacks current transaction.
    /// </summary>
    public Task RollbackAsync();

    /// <summary>
    /// Active connection.
    /// </summary>
    public IDbConnection Connection { get; }

    /// <summary>
    /// Active transaction.
    /// </summary>
    public IDbTransaction? Transaction { get; }
}