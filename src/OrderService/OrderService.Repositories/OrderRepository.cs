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
                                                "IdPrefix", "IdInfix",
                                                "IdPostfix", "CustomerId",
                                                "Items", "TotalAmount",
                                                "OrderStatus", "CreatedTime",
                                                "UpdatedTime")
                                         VALUES (
                                                 @IdPrefix, @IdInfix,
                                                 @IdPostfix, @CustomerId,
                                                 @Items, @TotalAmount,
                                                 @OrderStatus, @CreatedTime,
                                                 @UpdatedTime)
                                         RETURNING
                                                 "IdPrefix","IdInfix",
                                                 "IdPostfix", "CustomerId",
                                                 "Items", "TotalAmount",
                                                 "OrderStatus", "CreatedTime",
                                                 "UpdatedTime" 
                             """;

        return await _connection.QuerySingleOrDefaultAsync<OrderResponseItem>(query, order) 
               ?? throw new InvalidOperationException($"Unable to add order with ID:[{order.IdPrefix}-{order.IdInfix}-" +
                                                      $"{order.IdPostfix} for user:{order.CustomerId}]");
    }

    public async Task<OrderResponseItem> GetByIdAsync(long orderId)
    {
        throw new NotImplementedException();
    }
}