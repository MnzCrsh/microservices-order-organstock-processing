using System.Data;
using OrderService.Entities.Models.Entities;
using Dapper;
using OrderService.Repositories.Abstractions;

namespace OrderService.Repositories;

public class OrderRepository : IOrderRepository
{
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

        var res = await connection.QuerySingleOrDefaultAsync<Order>(query, order, transaction) ??
                  throw new ArgumentException($"Unable to create oder for customer with ID[{order.CustomerId}]");

        return res;
    }

    public async Task<bool> UpdateAsync(Order order, IDbConnection connection, IDbTransaction transaction)
    {
        const string query = """
                             UPDATE "Order"
                             SET "OrderStatus" = @OrderStatus, "UpdatedTime" = @UpdatedTime
                             WHERE "Id" = @Id
                             """;

        return await connection.ExecuteAsync(query, order, transaction) > 0;
    }

    public async Task<Order> GetByIdAsync(Guid orderId, IDbConnection connection, IDbTransaction transaction)
    {
        const string query = """
                             SELECT * FROM "Order" WHERE "Id" = @orderId;
                             """;

        var res = await connection.QuerySingleOrDefaultAsync<Order>(query, orderId, transaction)
                  ?? throw new ArgumentException($"Unable to find order with ID[{orderId}]");

        return res;
    }

    public async Task<IEnumerable<Guid>> GetTopThreeItems(IDbConnection connection, IDbTransaction transaction)
    {
        const string query = """
                             WITH ParsedJson AS (SELECT jsonb_array_elements("Items") ->> 'Id' AS item_id FROM "Order")
                             SELECT item_id, count(*) as count
                             FROM ParsedJson
                             GROUP BY item_id
                             ORDER BY count DESC
                             LIMIT 3
                             """;

        var res = await connection.QueryAsync<Guid>(query, transaction);
        return res;
    }
}