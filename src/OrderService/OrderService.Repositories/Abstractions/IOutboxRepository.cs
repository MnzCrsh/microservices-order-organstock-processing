using System.Data;
using OrderService.Entities;
using OrderService.Entities.Models.Entities;

namespace OrderService.Repositories.Abstractions;

/// <summary>
/// Transactional outbox repository
/// </summary>
public interface IOutboxRepository
{
    /// <summary>
    /// Asynchronously adds message to outbox
    /// </summary>
    /// <param name="outboxMessage">Message entity</param>
    /// <param name="connection">Sql connection</param>
    /// <param name="transaction">Sql transaction</param>
    Task<OutboxMessage> AddAsync(OutboxMessage outboxMessage, IDbConnection connection, IDbTransaction transaction);

    /// <summary>
    /// Asynchronously updates messages in outbox
    /// </summary>
    /// <param name="status">Status to set</param>
    /// <param name="connection">Sql connection</param>
    /// <param name="transaction">Sql transaction</param>
    /// <param name="ids">IDs of messages to update</param>
    Task<bool> UpdateAsync(IEnumerable<Guid> ids, MessageStatus status, IDbConnection connection, IDbTransaction transaction);

    /// <summary>
    /// Asynchronously fetches outbox messages by status
    /// </summary>
    /// <param name="batchSize">Limit of fetched messages</param>
    /// <param name="status">Status to fetch</param>
    /// <param name="connection">Sql connection</param>
    /// <param name="transaction">Sql transaction</param>
    Task<IEnumerable<OutboxMessage?>> FetchMessagesByStatusAsync(int batchSize, MessageStatus status,
        IDbConnection connection, IDbTransaction transaction);
}