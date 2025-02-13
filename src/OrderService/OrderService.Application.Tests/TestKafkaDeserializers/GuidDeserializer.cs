using Confluent.Kafka;

namespace OrderService.Application.Tests.TestKafkaDeserializers;

public class GuidDeserializer : IDeserializer<Guid>
{
    public Guid Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context) => new(data.ToArray());
}