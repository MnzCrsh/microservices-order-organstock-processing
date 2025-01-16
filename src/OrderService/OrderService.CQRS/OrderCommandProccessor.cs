using System.Text.Json;
using Confluent.Kafka;
using OrderService.Entities;
using OrderService.Entities.Models.Commands;
using OrderService.Entities.Models.Events;

namespace OrderService.CQRS;

public class OrderCommandProcessor(IProducer<string, string> producer) : IOrderCommandProcessor
{
    public async Task<Guid> ExecuteCreateOrderCommandAsync(CreateOrderCommand command)
    {
        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = Guid.NewGuid(),
            CustomerId = command.CustomerId,
            OrderDate = DateTimeOffset.Now,
            TotalAmount = command.TotalAmount,
            OrderStatus = OrderStatus.Created,
            Items = command.Items
        };

        var message = new Message<string, string>
        {
            Key = orderCreatedEvent.OrderId.ToString(),
            Value = JsonSerializer.Serialize(orderCreatedEvent)
        };

        message.Headers.Add("trace-id", orderCreatedEvent.OrderId.ToByteArray());
        
        await producer.ProduceAsync("order-events", message);
        
        return orderCreatedEvent.OrderId;
    }
}