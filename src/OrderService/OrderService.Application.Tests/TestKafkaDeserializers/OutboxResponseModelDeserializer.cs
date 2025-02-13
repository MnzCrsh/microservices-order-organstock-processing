using System.Text.Json;
using Confluent.Kafka;
using OrderService.Entities.Models.Responses;

namespace OrderService.Application.Tests.TestKafkaDeserializers;

public class OutboxResponseModelDeserializer : IDeserializer<OutboxResponseModel>
{
    public OutboxResponseModel Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return JsonSerializer.Deserialize<OutboxResponseModel>(data) ??
               throw new ArgumentException("Unable to deserialize the outbox message.");
    }
}