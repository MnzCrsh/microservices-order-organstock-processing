using System.Data;
using OrderService.Entities.Models.Entities;
using Dapper;
using Npgsql;
using OrderService.Repositories.Abstractions;

namespace OrderService.Repositories;

/// <inheritdoc/>
public class OrderRepository : IOrderRepository
{
    /// <inheritdoc/>
    public async Task<Order> AddAsync(Order order, IDbConnection connection, IDbTransaction transaction)
    {
        var (npgsqlConnection, npgsqlTransaction) = CheckConnectionType(connection, transaction);

        const string query = """
                             INSERT INTO "Order" (
                                                "Id","CustomerId",
                                                "Items", "TotalAmount",
                                                "OrderStatus", "CreatedTime",
                                                "UpdatedTime")
                                         VALUES (
                                                 @Id, @CustomerId,
                                                 @Items, @TotalAmount,
                                                 @OrderStatus, @CreatedTime,
                                                 @UpdatedTime)
                                         RETURNING
                                                 "Id", "CustomerId",
                                                 "Items", "TotalAmount",
                                                 "OrderStatus", "CreatedTime",
                                                 "UpdatedTime" 
                             """;

        var res = await npgsqlConnection.QuerySingleOrDefaultAsync<Order>(query, order, npgsqlTransaction) ??
                  throw new ArgumentException($"Unable to create oder for customer with ID[{order.CustomerId}]");

        return res;
    }
    
    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(Order order, IDbConnection connection, IDbTransaction transaction)
    {
        var (npgsqlConnection, npgsqlTransaction) = CheckConnectionType(connection, transaction);

        
        const string query = """
                             UPDATE "Order"
                             SET "OrderStatus" = @OrderStatus, "UpdatedTime" = @UpdatedTime
                             WHERE "Id" = @Id
                             """;

        return await npgsqlConnection.ExecuteAsync(query, order, npgsqlTransaction) > 0;
    }

    public async Task<Order> GetByIdAsync(Guid orderId, IDbConnection connection, IDbTransaction transaction)
    {
        var (npgsqlConnection, npgsqlTransaction) = CheckConnectionType(connection, transaction);
        
        const string query = """
                             SELECT * FROM "Order" WHERE "Id" = @orderId;
                             """;

        var res = await npgsqlConnection.QuerySingleOrDefaultAsync<Order>(query, orderId, npgsqlTransaction)
                  ?? throw new ArgumentException($"Unable to find order with ID[{orderId}]");

        return res;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Guid>> GetTopThreeItems(IDbConnection connection, IDbTransaction transaction)
    {
        var (npgsqlConnection, npgsqlTransaction) = CheckConnectionType(connection, transaction);

        const string query = """
                             WITH ParsedJson AS (SELECT jsonb_array_elements("Items") ->> 'Id' AS item_id FROM "Order")
                             SELECT item_id, count(*) as count
                             FROM ParsedJson
                             GROUP BY item_id
                             ORDER BY count DESC
                             LIMIT 3
                             """;

        var res = await npgsqlConnection.QueryAsync<Guid>(query, npgsqlTransaction);
        return res;
    }
    
    private static (NpgsqlConnection connection, NpgsqlTransaction transaction) CheckConnectionType(
        IDbConnection connection, IDbTransaction transaction)
    {
        if (connection is not NpgsqlConnection npgConnection)
            throw new InvalidOperationException($"Invalid connection type {typeof(NpgsqlConnection)}");

        if (transaction is not NpgsqlTransaction npgTransaction)
            throw new InvalidOperationException($"Invalid transaction type {typeof(NpgsqlTransaction)}");
        
        return (npgConnection, npgTransaction);
    }
}