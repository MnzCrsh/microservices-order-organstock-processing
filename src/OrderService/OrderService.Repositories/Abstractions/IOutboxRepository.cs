using System.Data;
using OrderService.Entities.Models.Entities;

namespace OrderService.Repositories.Abstractions;

public interface IOutboxRepository
{
    Task<OutboxMessage> AddAsync(OutboxMessage outboxMessage, IDbConnection connection, IDbTransaction transaction);

    Task<bool> UpdateAsync(OutboxMessage outboxMessage, IDbConnection connection, IDbTransaction transaction);

    Task<IEnumerable<OutboxMessage>?> FetchUnprocessedMessagesAsync(IDbConnection connection, IDbTransaction transaction);
}