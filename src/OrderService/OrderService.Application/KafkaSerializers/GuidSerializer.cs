using Confluent.Kafka;

namespace OrderService.Application.KafkaSerializers;

public class GuidSerializer : ISerializer<Guid>
{
    public byte[] Serialize(Guid data, SerializationContext context) => data.ToByteArray();
}