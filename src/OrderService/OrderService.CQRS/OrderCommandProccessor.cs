using Confluent.Kafka;
using OrderService.Entities.Models.Commands;

namespace OrderService.CQRS;

public class OrderCommandProcessor(IProducer<string, string> producer) : IOrderCommandProcessor
{
    public async Task<Guid> ExecuteCreateAsync(CreateOrderCommand command)
    {
        // var orderCreatedEvent = new OrderCreatedEvent
        // {
        //
        //     CustomerId = command.CustomerId,
        //     OrderDate = DateTimeOffset.Now,
        //     TotalAmount = command.TotalAmount,
        //     OrderStatus = OrderStatus.Created,
        //     Items = command.Items
        // };
        //
        // var message = new Message<string, string>
        // {
        //     Key = orderCreatedEvent.OrderId.ToString(),
        //     Value = JsonSerializer.Serialize(orderCreatedEvent)
        // };
        //
        // message.Headers.Add("trace-id", orderCreatedEvent.OrderId.ToByteArray());
        //
        // await producer.ProduceAsync("order-events", message);
        //
        // return orderCreatedEvent.OrderId;
        
        throw new NotImplementedException();
    }
}