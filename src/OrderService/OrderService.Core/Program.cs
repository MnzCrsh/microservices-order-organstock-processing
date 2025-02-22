using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using OrderService.Application;
using OrderService.CQRS;
using OrderService.gRPC;
using OrderService.Mapping;
using OrderService.OutboxDaemon;
using OrderService.Postgres;
using OrderService.Repositories.Helpers;
using OrderService.Validation;

var builder = WebApplication.CreateBuilder(args);

new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var kafkaConfig = builder.Configuration.GetSection("Kafka");
var postgresConfig = builder.Configuration.GetSection("Postgres");
var outboxConfig = builder.Configuration.GetSection("Outbox");
var sslConfig = builder.Configuration.GetSection("Ssl");

if (builder.Environment.EnvironmentName != "Test")
{
    builder.Services
        .AddPostgresMigrations(postgresConfig["ConnectionString"]!)
        .AddRepositoriesModule()
        .AddKafkaProducers(kafkaConfig)
        .AddMappingModule()
        .AddValidation()
        .AddApplicationServicesModule()
        .RegisterSqlConnection(postgresConfig)
        .RegisterOutboxConfig(outboxConfig)
        .AddOutboxDaemon()
        .AddGrpcModule(builder.Environment.IsDevelopment())
        .AddCqrsModule(builder.Configuration);
}

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5000, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });

    options.Listen(IPAddress.Any, 5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
        listenOptions.UseHttps(sslConfig["certPath"]!, sslConfig["password"]);
    });
});


var app = builder.Build();

app.Use(async (context, next) =>
{
    var allowedIps = builder.Configuration.GetSection("AllowedIps").Get<string[]>();
    var remoteIp = context.Connection.RemoteIpAddress?.ToString();

    if (allowedIps!.Contains(remoteIp))
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync("Forbidden");
        context.Abort();
        return;
    }

    await next();
});


app.UseRouting();


if (builder.Environment.EnvironmentName != "Test")
{
    app.Services.RunMigrations();
    app.MapGrpcService<OrderGrpcService>();
    app.MapHealthChecks("/health");
    app.UseHttpsRedirection();
}



app.Run();

/// <summary>
/// Used as work-around for tests
/// </summary>
public partial class Program;