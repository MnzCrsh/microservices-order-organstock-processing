using System.Data;
using OrderService.Entities.Models.Entities;
using OrderService.Entities.Models.Responses;
using Dapper;
using OrderService.Repositories.Abstractions;

namespace OrderService.Repositories;

public class OrderRepository : IOrderRepository
{
    public async Task<OrderResponseItem> AddAsync(Order order, IDbConnection connection, IDbTransaction transaction)
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

        var res = await connection.QuerySingleOrDefaultAsync<OrderResponseItem>(query, order) ?? 
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
        
        return await connection.ExecuteAsync(query, order) > 0;
    }

    public async Task<OrderResponseItem> GetByIdAsync(Guid orderId, IDbConnection connection, IDbTransaction transaction)
    {
        const string query = """
                             SELECT * FROM "Order" WHERE "Id" = @orderId;
                             """;
        
        var res = await connection.QuerySingleOrDefaultAsync<OrderResponseItem>(query, orderId) 
                  ?? throw new ArgumentException($"Unable to find order with ID[{orderId}]");
        
        return res;
    }
}