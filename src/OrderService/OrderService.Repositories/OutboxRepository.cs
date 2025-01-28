using Dapper;
using OrderService.Entities.Models;
using OrderService.Entities.Models.Responses;

namespace OrderService.Repositories;

public class OutboxRepository(IDbConnectionFactory connectionFactory) : IOutboxRepository
{
    
    
    public async Task<OutboxResponseModel> SaveAsync(OutboxMessage outboxMessage)
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
        using var connection = connectionFactory.CreateConnection();
        var res = await connection.QuerySingleOrDefaultAsync<OutboxResponseModel>(query, outboxMessage) ?? 
                  throw new ArgumentException($"Unable to save message with EventType:[{outboxMessage.EventType}]");
        
        return res;
    }

    public async Task<OutboxResponseModel> UpdateAsync(OutboxMessage outboxMessage)
    {
        const string query = """
                             UPDATE "Outbox"
                             SET "Status" = @Status, "ProcessedTime" = @ProcessedTime
                             WHERE "Id" = @Id
                             """;
        
        using var connection = connectionFactory.CreateConnection();
        var res = await connection.QuerySingleOrDefaultAsync<OutboxResponseModel>(query, outboxMessage) ?? 
                  throw new ArgumentException($"Unable to save message with EventType:[{outboxMessage.EventType}]");
        
        return res;
    }

    public async Task<IEnumerable<OutboxResponseModel>?> FetchUnprocessedMessagesAsync()
    {
        const string query = """
                             SELECT * FROM "Outbox" WHERE "Status" = 1 LIMIT 250;
                             """;
        
        using var connection = connectionFactory.CreateConnection();
        var res = await connection.QuerySingleOrDefaultAsync<IEnumerable<OutboxResponseModel>>(query);
        
        return res;
    }
}