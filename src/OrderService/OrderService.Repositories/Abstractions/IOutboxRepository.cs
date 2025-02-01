using System.Data;
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
    /// Asynchronously updates message in outbox
    /// </summary>
    /// <param name="outboxMessage">Message entity</param>
    /// <param name="connection">Sql connection</param>
    /// <param name="transaction">Sql transaction</param>
    Task<bool> UpdateAsync(OutboxMessage outboxMessage, IDbConnection connection, IDbTransaction transaction);

    /// <summary>
    /// Asynchronously fetches unprocessed outbox messages
    /// </summary>
    /// <param name="connection">Sql connection</param>
    /// <param name="transaction">Sql transaction</param>
    Task<IEnumerable<OutboxMessage>?> FetchUnprocessedMessagesAsync(IDbConnection connection, IDbTransaction transaction);
}