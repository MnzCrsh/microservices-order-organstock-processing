using System.Data;
using OrderService.Entities.Models.Entities;
using Dapper;
using OrderService.Repositories.Abstractions;

namespace OrderService.Repositories;

/// <inheritdoc/>
public class OrderRepository : IOrderRepository
{
    /// <inheritdoc/>
    public async Task<Order> AddAsync(Order order, IDbConnection connection, IDbTransaction transaction)
    {
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


        var res = await connection.QuerySingleOrDefaultAsync<Order>(query, order, transaction)
                  ?? throw new ArgumentException($"Unable to create order for customer with ID[{order.CustomerId}]");

        return res;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(Order order, IDbConnection connection, IDbTransaction transaction)
    {
        const string query = """
                             UPDATE "Order"
                             SET "OrderStatus" = @OrderStatus, "UpdatedTime" = @UpdatedTime
                             WHERE "Id" = @Id
                             """;

        return await connection.ExecuteAsync(query, order, transaction) > 0;
    }

    public async Task<Order> GetByIdAsync(Guid orderId, IDbConnection connection)
    {
        const string query = """
                             SELECT * FROM "Order" WHERE "Id" = @orderId;
                             """;

        var res = await connection.QuerySingleOrDefaultAsync<Order>(query, new { orderId })
                  ?? throw new ArgumentException($"Unable to find order with ID[{orderId}]");

        return res;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Guid>> GetTopThreeItems(IDbConnection connection)
    {
        const string query = """
                             WITH ParsedJson AS (
                             SELECT 
                                 CAST(elem AS UUID) AS item_id 
                             FROM "Order"
                             LEFT JOIN LATERAL jsonb_array_elements_text(
                                 CASE 
                                     WHEN jsonb_typeof("Items"::jsonb) = 'array' 
                                     THEN "Items"::jsonb 
                                     ELSE '[]'::jsonb 
                                 END
                                ) AS elem ON true
                             )
                             SELECT item_id
                             FROM ParsedJson
                             WHERE item_id IS NOT NULL
                             GROUP BY item_id
                             ORDER BY COUNT(*) DESC
                             LIMIT 3
                             """;

        var res = await connection.QueryAsync<Guid>(query);

        return res;
    }
}