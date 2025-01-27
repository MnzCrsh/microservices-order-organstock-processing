using System.Data;
using OrderService.Entities.Models.Entities;
using OrderService.Entities.Models.Responses;
using Dapper;
using Microsoft.Data.SqlClient;

namespace OrderService.Repositories;

public class OrderRepository(DbConfig config) : IOrderRepository
{
    private readonly IDbConnection _connection = new SqlConnection(config.ConnectionString);

    public async Task<OrderResponseItem> AddAsync(Order order)
    {
        const string query = """
                              INSERT INTO "Order" (
                                                 "IdInfix","CustomerId",
                                                 "Items", "TotalAmount",
                                                 "OrderStatus", "CreatedTime",
                                                 "UpdatedTime")
                                          VALUES (
                                                  @IdInfix, @CustomerId,
                                                  @Items, @TotalAmount,
                                                  @OrderStatus, @CreatedTime,
                                                  @UpdatedTime)
                                          RETURNING
                                                  "IdInfix", "CustomerId",
                                                  "Items", "TotalAmount",
                                                  "OrderStatus", "CreatedTime",
                                                  "UpdatedTime" 
                              """;

        return await _connection.QuerySingleOrDefaultAsync<OrderResponseItem>(query, order) ??
               throw new ArgumentException($"Unable to create oder for customer with ID[{order.CustomerId}]");
    }

    public async Task<OrderResponseItem> GetByIdAsync(long orderId)
    {
        const string query = """
                             SELECT * FROM "Order" WHERE "CombinedKey" = @orderId;
                             """;
        
        return await _connection.QuerySingleOrDefaultAsync<OrderResponseItem>(query, orderId) 
               ?? throw new ArgumentException($"Unable to find order with ID[{orderId}]");
    }
}