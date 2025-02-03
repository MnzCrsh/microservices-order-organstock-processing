using OrderService.Application;
using OrderService.CQRS;
using OrderService.Mapping;
using OrderService.OutboxDaemon;
using OrderService.Postgres;
using OrderService.Repositories.Helpers;

var builder = WebApplication.CreateBuilder(args);

new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var kafkaConfig = builder.Configuration.GetSection("Kafka");
var postgresConfig = builder.Configuration.GetSection("Postgres");
var outboxConfig = builder.Configuration.GetSection("Outbox");

builder.Services
    .AddOpenApi()
    .AddMigrations(postgresConfig["ConnectionString"])
    .AddRepositoriesModule(postgresConfig)
    .AddKafkaProducers(kafkaConfig)
    .AddMappingModule()
    .AddApplicationServicesModule(outboxConfig)
    .AddOutboxDaemon()
    .AddCqrs(builder.Configuration);


var app = builder.Build();

app.Services.RunMigrations();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.Run();

