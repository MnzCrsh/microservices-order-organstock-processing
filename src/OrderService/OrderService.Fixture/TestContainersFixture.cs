using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Docker.DotNet;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Postgres.Migrations;
using Testcontainers.PostgreSql;
using Testcontainers.Kafka;
using Xunit;
using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace OrderService.Fixture;

/// <summary>
/// Test containers based fixture for integration testing
/// </summary>
public class TestContainersFixture : IAsyncLifetime
{

    #region PostgreSql

    private const string PostgresUsername = "postgres";
    private const string PostgresImage = "postgres:latest";
    private const int PostgresPort = 5432;
    private readonly string _postgresDb = $"Postgres_{Guid.CreateVersion7()}";
    private readonly PostgreSqlContainer _postgresContainer;

    #endregion

    #region ApacheKafka

    private const string KafkaImage = "confluentinc/cp-kafka:latest";
    private const int KafkaPort = 9092;
    private readonly KafkaContainer _kafkaContainer;

    #endregion

    private readonly INetwork _network;

    /// <summary>
    /// Public postgres connection string
    /// </summary>
    public string PostgresConnectionString { get; private set; } = null!;

    /// <summary>
    /// Public kafka bootstrap server address
    /// </summary>
    public string KafkaBootstrapServer { get; private set; } = null!;

    public TestContainersFixture()
    {
        _network = new NetworkBuilder().Build();

        _postgresContainer = BuildPostgresContainer(_network);
        _kafkaContainer = BuildKafkaContainer(_network);
    }

    /// <summary>
    /// Postgres container build configuration
    /// </summary>
    /// <param name="privateNetwork">Private network instance</param>
    private PostgreSqlContainer BuildPostgresContainer(INetwork privateNetwork) =>
        new PostgreSqlBuilder()
            .WithImage(PostgresImage)
            .WithPortBinding(PostgresPort, assignRandomHostPort: true)
            .WithNetwork(privateNetwork)
            .WithNetworkAliases("postgres")
            .WithDatabase(_postgresDb)
            .WithUsername(PostgresUsername)
            .WithPassword(PostgresUsername)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(PostgresPort))
            .WithCleanUp(true)
            .Build();

    /// <summary>
    /// Kafka container build configuration
    /// </summary>
    /// <param name="privateNetwork">Private network instance</param>
    private KafkaContainer BuildKafkaContainer(INetwork privateNetwork) =>
        new KafkaBuilder()
            .WithImage(KafkaImage)
            .WithPortBinding(KafkaPort, assignRandomHostPort: true)
            .WithNetwork(privateNetwork)
            .WithNetworkAliases("kafka")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(KafkaPort))
            .WithCleanUp(true)
            .Build();

    /// <summary>
    /// Executes SQL data seeding file inside PostgreSql container
    /// </summary>
    private async Task ExecuteSeedSqlData()
    {
        var sqlQuery = await File.ReadAllTextAsync(Path.Combine(Environment.CurrentDirectory, "seed_data.sql"));
        var queryFilePath =
            string.Join("/temp/", Guid.CreateVersion7().ToString(), "seed_data");

        await _postgresContainer.CopyAsync(Encoding.UTF8.GetBytes(sqlQuery), queryFilePath);

        var commandRes = await _postgresContainer.ExecAsync([
            "psql",
            "--username", PostgresUsername,
            "--dbname", _postgresDb,
            "--file", queryFilePath
        ]);

        if (!string.IsNullOrEmpty(commandRes.Stderr))
        {
            throw new Exception($"Failed to run shell script with: {commandRes.Stderr}");
        }
    }

    /// <summary>
    /// Initializes kafka topics
    /// </summary>
    private async Task InitializeKafkaTopics()
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = _kafkaContainer.GetBootstrapAddress()
        }).Build();

        try
        {
            await adminClient.CreateTopicsAsync(
            [
                new TopicSpecification
               {
                   Name = "order-outbox-service",
                   NumPartitions = 1,
                   ReplicationFactor = 1
               }
            ]);
        }
        catch (CreateTopicsException ex) when (ex.Results.Any(r => r.Error.Code == ErrorCode.TopicAlreadyExists))
        {
            // Topic already exist, so we will just suppress the exception
        }
    }

    /// <summary>
    /// Initializes test containers with errors extracting from failed container
    /// </summary>
    private async Task InitializeContainersWithLErrorTracing()
    {
        var containers = new List<IContainer> { _postgresContainer, _kafkaContainer };

        try
        {
            var containerTasks = containers.Select(async container => await container.StartAsync());
            await Task.WhenAll(containerTasks);
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

    /// <summary>
    /// Creates enriched exception with formated logs from container 
    /// </summary>
    /// <param name="failedContainer">Instance of failed container</param>
    /// <param name="logs">Container logs</param>
    private static InvalidOperationException CreateContainerException(IContainer failedContainer,
        (string _, string Errors) logs)
    {
        var message = new StringBuilder($"Failed to start container:[{failedContainer.Image.FullName}] with:");

        var logLines = logs.Errors.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
        if (logLines.Length == 0)
        {
            message.AppendLine("Unable to fetch container errors.");
        }
        foreach (var line in logLines)
        {
            message.AppendLine($"\n \t -> Error:{line}");
        }

        return new InvalidOperationException(message.ToString());
    }

    /// <summary>
    /// Applies FluentMigrator migrations
    /// </summary>
    private void ApplyMigrations()
    {
        var serviceProvider = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(_postgresContainer.GetConnectionString())
                .ScanIn(typeof(AddOrderAndOutboxTable).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        runner.MigrateUp();
    }

    /// <inheritdoc/>
    public async Task InitializeAsync()
    {
        await _network.CreateAsync();

        await InitializeContainersWithLErrorTracing();

        ApplyMigrations();

        await ExecuteSeedSqlData();

        await InitializeKafkaTopics();

        PostgresConnectionString = _postgresContainer.GetConnectionString();
        KafkaBootstrapServer = _kafkaContainer.GetBootstrapAddress();
    }

    /// <inheritdoc/>
    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
        await _kafkaContainer.DisposeAsync();

        await _network.DisposeAsync();
    }
}
