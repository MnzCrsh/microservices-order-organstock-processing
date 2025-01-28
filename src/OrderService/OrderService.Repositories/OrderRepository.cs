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

        var res = await _connection.QuerySingleOrDefaultAsync<OrderResponseItem>(query, order) ?? 
                  throw new ArgumentException($"Unable to create oder for customer with ID[{order.CustomerId}]");
        
        return res;
    }

    public async Task<OrderResponseItem> GetByIdAsync(Guid orderId)
    {
        const string query = """
                             SELECT * FROM "Order" WHERE "Id" = @orderId;
                             """;
        
        var res = await _connection.QuerySingleOrDefaultAsync<OrderResponseItem>(query, orderId) 
                  ?? throw new ArgumentException($"Unable to find order with ID[{orderId}]");
        
        return res;
    }
}