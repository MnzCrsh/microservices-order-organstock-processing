using System.Text;
using Docker.DotNet;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using Testcontainers.PostgreSql;
using Testcontainers.Kafka;
using Xunit;
using IContainer = DotNet.Testcontainers.Containers.IContainer;

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

    private async Task ExecuteSeedSqlData()
    {
        var sqlQuery = await File.ReadAllTextAsync(Path.Combine(Environment.CurrentDirectory, "seed_data.sql"));
        await _postgresContainer.ExecScriptAsync(sqlQuery);
    }

    private async Task InitializeContainersWithLErrorTracing()
    {
        var containers = new List<IContainer> { _postgresContainer, _kafkaContainer };

        try
        {
            foreach (var container in containers)
            {
                await container.StartAsync();
            }
        }
        catch (DockerApiException dockerEx)
        {
            var match = FixtureRegexs.MatchDockerId(dockerEx.Message);
            if (!match.Success)
            {
                throw;
            }

            var containerId = match.Groups[1].Value;
            var failedContainer = containers.FirstOrDefault(x => x.Id == containerId);
            if (failedContainer == null)
            {
                throw;
            }

            var logs = await failedContainer.GetLogsAsync();
            var containerException = CreateContainerException(failedContainer, logs);

            throw new AggregateException(dockerEx, containerException);
        }
    }

    private static InvalidOperationException CreateContainerException(IContainer failedContainer,
        (string _, string Errors) logs)
    {
        var message = new StringBuilder($"Failed to start container:[{failedContainer.Image.FullName}] with:");
        var logLines = logs.Errors.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in logLines)
        {
            message.AppendLine($"\n \t -> Error:{line}");
        }
        return new InvalidOperationException(message.ToString());
    }

    public async Task InitializeAsync()
    {
        await _network.CreateAsync();

        await InitializeContainersWithLErrorTracing();

        await ExecuteSeedSqlData();

        PostgresConnectionString = _postgresContainer.GetConnectionString();
        KafkaBootstrapServer = _kafkaContainer.GetBootstrapAddress();
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
        await _kafkaContainer.DisposeAsync();
        await _network.DisposeAsync();
    }
}
