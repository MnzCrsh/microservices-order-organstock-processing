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
    IUnitOfWork unitOfWork,
    IMapperFactory mapperFactory,
    ILogger<OrderService> logger) : IOrderService
{
    /// <inheritdoc/>
    public async Task<OrderResponseItem> CreateAsync(CreateOrderCommand command)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();

            var orderResult = await ExecuteCreateOrder(command, unitOfWork.Connection, unitOfWork.Transaction!);

            await ExecuteCreateOutboxMessage(command, unitOfWork.Connection, unitOfWork.Transaction!);

            await unitOfWork.CommitAsync();

            var orderResponseMapper = mapperFactory.GetMapper<Order, OrderResponseItem>();
            return orderResponseMapper.Map(orderResult ?? throw new InvalidOperationException
                ($"Unable to add Order with CustomerId[{command.CustomerId}]."));
        }
        catch (Exception e)
        {
            logger.LogError("An error occured while adding new Order. {Error}", e);
            await unitOfWork.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(UpdateOrderCommand command)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();

            var updateResult = await ExecuteUpdateOrder(command, unitOfWork.Connection, unitOfWork.Transaction!);

            await ExecuteUpdateCreateOutboxMessage(command, unitOfWork.Connection, unitOfWork.Transaction!);

            await unitOfWork.CommitAsync();

            return updateResult;
        }
        catch (Exception e)
        {
            logger.LogError("An error occured while updating Order with ID[{Id}]. {Error}", command.Id, e);
            await unitOfWork.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<OrderResponseItem> GetByIdAsync(Guid id)
    {
        try
        {
            await unitOfWork.OpenConnectionAsync();

            var res = await orderRepository.GetByIdAsync(id, unitOfWork.Connection);

            var orderResponseMapper = mapperFactory.GetMapper<Order, OrderResponseItem>();
            return orderResponseMapper.Map(res);
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
            await unitOfWork.OpenConnectionAsync();

            var res = await orderRepository.GetTopThreeItems(unitOfWork.Connection);

            return res.ToArray();
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