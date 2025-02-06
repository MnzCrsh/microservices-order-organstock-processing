using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using Testcontainers.PostgreSql;
using Xunit;

namespace OrderService.Fixture;

public class TestContainersFixture : IAsyncLifetime
{
    private const string DatabaseName = "your_database";
    private const string Username = "your_username";
    private const string PostgresImage = "postgres:latest";
    private const int PostgresPort = 5432;

    private readonly INetwork _network;
    private readonly PostgreSqlContainer _postgresContainer;
    public static string PostgresConnectionString { get; private set; } = null!;

    public TestContainersFixture()
    {
        _network = new NetworkBuilder().Build();
        _postgresContainer = BuildPostgresContainer(_network);
    }

    private static PostgreSqlContainer BuildPostgresContainer(INetwork? privateNetwork) =>
        new PostgreSqlBuilder()
            .WithImage(PostgresImage)
            .WithPortBinding(PostgresPort, assignRandomHostPort: true)
            .WithNetwork(privateNetwork)
            .WithNetworkAliases("postgres")
            .WithEnvironment("POSTGRES_DB", DatabaseName)
            .WithEnvironment("POSTGRES_USER", Username)
            .WithEnvironment("POSTGRES_PASSWORD", Username)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(PostgresPort))
            .WithCleanUp(true)
            .Build();

    public async Task InitializeAsync()
    {
        await _network.CreateAsync();
        await _postgresContainer.StartAsync();

        PostgresConnectionString =
            $"Server=localhost;Database={DatabaseName};Log Parameters=true;Port={_postgresContainer.GetMappedPublicPort(PostgresPort)};" +
            $"Username={Username};Password={Username};Include Error Detail=true;";
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
        await _network.DisposeAsync();
    }
}