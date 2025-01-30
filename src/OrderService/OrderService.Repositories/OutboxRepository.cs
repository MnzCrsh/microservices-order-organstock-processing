using System.Data;
using Dapper;
using OrderService.Entities.Models.Entities;
using OrderService.Repositories.Abstractions;

namespace OrderService.Repositories;

public class OutboxRepository : IOutboxRepository
{
    public async Task<OutboxMessage> AddAsync(OutboxMessage outboxMessage, IDbConnection connection, IDbTransaction transaction)
    {
        const string query = """
                             INSERT INTO "Outbox" (
                                                   "Id", "EventType",
                                                   "Payload", "CreatedTime",
                                                   "ProcessedTime")
                                          VALUES (
                                                   @Id, @EventType,
                                                   @Payload, @CreatedTime,
                                                   @ProcessedTime)
                                          RETURNING 
                                                   "Id", "EventType",
                                                   "Payload", "ProcessedTime",
                                                   "Status"
                             """;

        var res = await connection.QuerySingleOrDefaultAsync<OutboxMessage>(query, outboxMessage, transaction) ??
                  throw new ArgumentException($"Unable to save message with EventType:[{outboxMessage.EventType}]");

        return res;
    }

    public async Task<bool> UpdateAsync(OutboxMessage outboxMessage, IDbConnection connection, IDbTransaction transaction)
    {
        const string query = """
                             UPDATE "Outbox"
                             SET "Status" = @Status, "ProcessedTime" = @ProcessedTime
                             WHERE "Id" = @Id
                             """;

        return await connection.ExecuteAsync(query, outboxMessage, transaction) > 0;
    }

    public async Task<IEnumerable<OutboxMessage>?> FetchUnprocessedMessagesAsync(IDbConnection connection, IDbTransaction transaction)
    {
        const string query = """
                             SELECT * FROM "Outbox" WHERE "Status" = 1 LIMIT 250;
                             """;

        var res = await connection.QueryAsync<OutboxMessage>(query, transaction);

        return res;
    }
}