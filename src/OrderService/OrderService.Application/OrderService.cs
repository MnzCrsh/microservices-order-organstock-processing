using System.Data;
using Microsoft.Extensions.Logging;
using OrderService.Application.Abstractions;
using OrderService.Entities.Models.Commands;
using OrderService.Entities.Models.Entities;
using OrderService.Entities.Models.Responses;
using OrderService.Mapping;
using OrderService.Repositories.Abstractions;

namespace OrderService.Application;

public class OrderService(IOrderRepository orderRepository,
    IOutboxRepository outboxRepository,
    ITransactionHandler transaction,
    IMapperFactory mapperFactory,
    ILogger<OrderService> logger) : IOrderService
{
    /// <inheritdoc/>
    public async Task<OrderResponseItem> CreateAsync(CreateOrderCommand command)
    {
        try
        {
            var transactionRes = await transaction.ExecuteAsync(async (connection, dbTransaction) =>
            {
                var orderResult = await ExecuteCreateOrder(command, connection, dbTransaction);

                await ExecuteCreateOutboxMessage(command, connection, dbTransaction);

                return orderResult;
            });

            var orderResponseMapper = mapperFactory.GetMapper<Order, OrderResponseItem>();
            return orderResponseMapper.Map(transactionRes ?? throw new InvalidOperationException
                ($"Unable to add Order with CustomerId[{command.CustomerId}]."));
        }
        catch (Exception e)
        {
            logger.LogError("An error occured while adding new Order. {Error}", e);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(UpdateOrderCommand command)
    {
        try
        {
            var transactionRes = await transaction.ExecuteAsync(async (connection, dbTransaction) =>
            {
                var updateResult = await ExecuteUpdateOrder(command, connection, dbTransaction);

                await ExecuteUpdateCreateOutboxMessage(command, connection, dbTransaction);

                return updateResult;
            });

            return transactionRes;
        }
        catch (Exception e)
        {
            logger.LogError("An error occured while updating Order with ID[{Id}]. {Error}", command.Id, e);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<OrderResponseItem> GetByIdAsync(Guid id)
    {
        try
        {
            var transactionRes = await transaction.ExecuteAsync(async (connection, dbTransaction)
                => await orderRepository.GetByIdAsync(id, connection, dbTransaction));

            var orderResponseMapper = mapperFactory.GetMapper<Order, OrderResponseItem>();
            return orderResponseMapper.Map(transactionRes);
        }
        catch (Exception e)
        {
            logger.LogError("An error occured while fetching Order with ID:[{Id}]. {Error}", id, e);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Guid[]> GetTopThreeAsync()
    {
        try
        {
            var transactionRes = await transaction.ExecuteAsync(async (connection, dbTransaction)
                => await orderRepository.GetTopThreeItems(connection, dbTransaction));

            return transactionRes.ToArray();
        }
        catch (Exception e)
        {
            logger.LogError("An error occured while fetching Top 3 Ordered items. {Error}", e);
            throw;
        }
    }

    private async Task ExecuteCreateOutboxMessage(CreateOrderCommand command, IDbConnection connection,
        IDbTransaction dbTransaction)
    {
        var outboxMapper = mapperFactory.GetMapper<CreateOrderCommand, OutboxMessage>();
        var message = outboxMapper.Map(command);
        await outboxRepository.AddAsync(message, connection, dbTransaction);
    }

    private async Task<Order> ExecuteCreateOrder(CreateOrderCommand command, IDbConnection connection, IDbTransaction dbTransaction)
    {
        var orderMapper = mapperFactory.GetMapper<CreateOrderCommand, Order>();
        var order = orderMapper.Map(command);
        var orderResult = await orderRepository.AddAsync(order, connection, dbTransaction);
        return orderResult;
    }

    private async Task<bool> ExecuteUpdateOrder(UpdateOrderCommand command, IDbConnection connection,
        IDbTransaction dbTransaction)
    {
        var updateOrderMapper = mapperFactory.GetMapper<UpdateOrderCommand, Order>();
        var orderUpdate = updateOrderMapper.Map(command);
        var updateResult = await orderRepository.UpdateAsync(orderUpdate, connection, dbTransaction);
        return updateResult;
    }

    private async Task ExecuteUpdateCreateOutboxMessage(UpdateOrderCommand command, IDbConnection connection,
        IDbTransaction dbTransaction)
    {
        var updateOutboxMapper = mapperFactory.GetMapper<UpdateOrderCommand, OutboxMessage>();
        var outboxUpdate = updateOutboxMapper.Map(command);
        await outboxRepository.AddAsync(outboxUpdate, connection, dbTransaction);
    }
}