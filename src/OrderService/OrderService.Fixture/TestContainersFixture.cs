using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using Testcontainers.PostgreSql;
using Testcontainers.Kafka;
using Xunit;

namespace OrderService.Fixture;

public class TestContainersFixture : IAsyncLifetime
{
    private const string PostgresUsername = "postgres";
    private const string PostgresImage = "postgres:latest";
    private const int PostgresPort = 5432;

    private const string KafkaImage = "bitnami/kafka:latest";
    private const int KafkaPort = 9092;

    private readonly INetwork _network;
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly KafkaContainer _kafkaContainer;


    public string PostgresConnectionString { get; private set; } = null!;
    public string KafkaBootstrapServer { get; private set; } = null!;

    public TestContainersFixture()
    {
        _network = new NetworkBuilder().Build();
        _postgresContainer = BuildPostgresContainer(_network);
        _kafkaContainer = BuildKafkaContainer(_network);
    }

    private static PostgreSqlContainer BuildPostgresContainer(INetwork privateNetwork) =>
        new PostgreSqlBuilder()
            .WithImage(PostgresImage)
            .WithPortBinding(PostgresPort, assignRandomHostPort: true)
            .WithNetwork(privateNetwork)
            .WithNetworkAliases("postgres")
            .WithDatabase($"Postgres_{Guid.NewGuid()}")
            .WithUsername(PostgresUsername)
            .WithPassword(PostgresUsername)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(PostgresPort))
            .WithCleanUp(true)
            .Build();

    private static KafkaContainer BuildKafkaContainer(INetwork privateNetwork) =>
        new KafkaBuilder()
            .WithImage(KafkaImage)
            .WithPortBinding(KafkaPort, assignRandomHostPort: true)
            .WithNetwork(privateNetwork)
            .WithNetworkAliases("kafka")
            .WithEnvironment("KAFKA_AUTO_CREATE_TOPICS_ENABLE", "true")
            .WithEnvironment("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", "PLAINTEXT:PLAINTEXT,BROKER:PLAINTEXT")
            .WithEnvironment("KAFKA_LISTENERS", "PLAINTEXT://:29092,BROKER://:9092")
            .WithEnvironment("KAFKA_ADVERTISED_LISTENERS", "PLAINTEXT://kafka:29092,BROKER://localhost:9093")
            .WithEnvironment("KAFKA_INTER_BROKER_LISTENER_NAME", "PLAINTEXT")
            .WithEnvironment("KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR", "1")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(KafkaPort))
            .WithCleanUp(true)
            .Build();

    public async Task InitializeAsync()
    {
        await _network.CreateAsync();
        await _postgresContainer.StartAsync();

        PostgresConnectionString = _postgresContainer.GetConnectionString();
        KafkaBootstrapServer = _kafkaContainer.GetBootstrapAddress();
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
        await _network.DisposeAsync();
    }
}
