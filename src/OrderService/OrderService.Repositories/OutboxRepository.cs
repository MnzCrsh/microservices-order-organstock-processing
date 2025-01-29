using System.Data;
using Dapper;
using OrderService.Entities.Models;
using OrderService.Entities.Models.Responses;
using OrderService.Repositories.Abstractions;

namespace OrderService.Repositories;

public class OutboxRepository : IOutboxRepository
{
    public async Task<OutboxResponseModel> SaveAsync(OutboxMessage outboxMessage, IDbConnection connection, IDbTransaction transaction)
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

        var res = await connection.QuerySingleOrDefaultAsync<OutboxResponseModel>(query, outboxMessage, transaction) ??
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

    public async Task<IEnumerable<OutboxResponseModel>?> FetchUnprocessedMessagesAsync(IDbConnection connection, IDbTransaction transaction)
    {
        const string query = """
                             SELECT * FROM "Outbox" WHERE "Status" = 1 LIMIT 250;
                             """;

        var res = await connection.QueryAsync<OutboxResponseModel>(query, transaction);

        return res;
    }
}