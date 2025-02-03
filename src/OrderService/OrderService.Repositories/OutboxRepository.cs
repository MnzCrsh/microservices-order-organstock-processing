using System.Data;
using Dapper;
using OrderService.Entities;
using OrderService.Entities.Models.Entities;
using OrderService.Repositories.Abstractions;

namespace OrderService.Repositories;

/// <inheritdoc/>
public class OutboxRepository : IOutboxRepository
{
    /// <inheritdoc/>
    public async Task<OutboxMessage> AddAsync(OutboxMessage outboxMessage,
        IDbConnection connection, IDbTransaction transaction)
    {
        const string query = """
                             INSERT INTO "Outbox" (
                                                   "Id", "EventType",
                                                   "Payload", "CreatedTime")
                                          VALUES (
                                                   @Id, @EventType,
                                                   @Payload, @CreatedTime)
                                          RETURNING 
                                                   "Id", "EventType",
                                                   "Payload", "CreatedTime",
                                                   "Status"
                             """;

        var res = await connection.QuerySingleOrDefaultAsync<OutboxMessage>(query, outboxMessage, transaction) ??
                  throw new ArgumentException($"Unable to save message with EventType:[{outboxMessage.EventType}]");

        return res;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(IEnumerable<Guid> ids, MessageStatus status, IDbConnection connection, IDbTransaction transaction)
    {
        const string query = """
                             UPDATE "Outbox" AS o
                             SET 
                                 "Status" = @Status,
                                 "ProcessedTime" = NOW()
                             FROM (
                                SELECT
                                    unnest(@Ids) AS "Id",
                                    unnest(@Statuses) AS "Status"
                                 ) AS m
                             WHERE o."Id" = m."Id"
                             """;

        ids = ids.ToList();
        return await connection.ExecuteAsync(query, (ids, status), transaction) == ids.Count();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<OutboxMessage?>> FetchMessagesByStatusAsync(int batchSize, MessageStatus status,
        IDbConnection connection, IDbTransaction transaction)
    {
        const string query = """
                             WITH locked_messages AS (
                                 SELECT "Id"
                                 FROM "Outbox"
                                 WHERE "Status" = @Status
                                 ORDER BY "CreatedTime" 
                                 LIMIT @batchSize
                                 FOR UPDATE SKIP LOCKED
                             )                             
                             UPDATE "Outbox" o 
                             SET 
                                 "Status" = 2 --Processing
                             FROM locked_messages
                             where o."Id" = locked_messages."Id"
                             RETURNING o.*
                             """;

        var res = await connection.QueryAsync<OutboxMessage>(query, new { batchSize, status }, transaction);

        return res;
    }
}