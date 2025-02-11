using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using Testcontainers.PostgreSql;
using Xunit;

namespace OrderService.Fixture;

public class TestContainersFixture : IAsyncLifetime
{
    private const string PostgresUsername = "postgres";
    private const string PostgresImage = "postgres:latest";
    private const int PostgresPort = 5432;

    private readonly INetwork _network;
    private readonly PostgreSqlContainer _postgresContainer;
    public string PostgresConnectionString { get; private set; } = null!;

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
            .WithDatabase($"Postgres_{Guid.NewGuid()}")
            .WithUsername(PostgresUsername)
            .WithPassword(PostgresUsername)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(PostgresPort))
            .WithCleanUp(true)
            .Build();

    public async Task InitializeAsync()
    {
        await _network.CreateAsync();
        await _postgresContainer.StartAsync();

        PostgresConnectionString = _postgresContainer.GetConnectionString();
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
        await _network.DisposeAsync();
    }
}