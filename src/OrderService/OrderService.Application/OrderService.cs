using Microsoft.Extensions.Logging;
using OrderService.Entities.Models.Commands;
using OrderService.Entities.Models.Entities;
using OrderService.Entities.Models.Responses;
using OrderService.Mapping;
using OrderService.Repositories.Abstractions;
using OrderService.Repositories.Helpers;

namespace OrderService.Application;

public class OrderService(IOrderRepository orderRepository,
    IOutboxRepository outboxRepository
    , TransactionHandler transaction,
    IMapper<CreateOrderCommand, Order> orderCommandMapper,
    IMapper<CreateOrderCommand, OutboxMessage> outboxMapper,
    IMapper<Order, OrderResponseItem> orderResponseMapper,
    ILogger<OrderService> logger) : IOrderService
{
    public async Task<OrderResponseItem> CreateAsync(CreateOrderCommand command)
    {
        try
        {
            var order = orderCommandMapper.Map(command);
            var message = outboxMapper.Map(command);

            var transactionRes = await transaction.ExecuteAsync(async (connection, dbTransaction) =>
            {
                var orderResult = await orderRepository.AddAsync(order, connection, dbTransaction);
                await outboxRepository.AddAsync(message, connection, dbTransaction);
                return orderResult;
            });

            return orderResponseMapper.Map(transactionRes ?? throw new InvalidOperationException
                ($"Unable to add Order with CustomerId[{command.CustomerId}]."));
        }
        catch (Exception e)
        {
            logger.LogError("An error occured while adding new Order. {1}", e);
            throw;
        }
    }

    public async Task<OrderResponseItem> GetByIdAsync(Guid id)
    {
        try
        {
            var transactionRes = await transaction.ExecuteAsync(async (connection, dbTransaction)
                => await orderRepository.GetByIdAsync(id, connection, dbTransaction));

            return orderResponseMapper.Map(transactionRes);
        }
        catch (Exception e)
        {
            logger.LogError("An error occured while fetching Order with ID:[{1}]. {2}", id, e);
            throw;
        }
    }

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
            logger.LogError("An error occured while fetching Top 3 Ordered items. {1}", e);
            throw;
        }
    }
}