using System.Text.Json;
using Confluent.Kafka;
using OrderService.Entities.Models.Responses;

namespace OrderService.Application.KafkaSerializers;

public class OutboxResponseModelSerializer : ISerializer<OutboxResponseModel>
{
    public byte[] Serialize(OutboxResponseModel data, SerializationContext context) =>
        JsonSerializer.SerializeToUtf8Bytes(data);
}