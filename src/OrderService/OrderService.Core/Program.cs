using OrderService.Application;
using OrderService.Core;
using OrderService.CQRS;
using OrderService.Mapping;
using OrderService.Postgres;
using OrderService.Repositories.Helpers;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
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
    .AddCqrs(builder.Configuration);


var app = builder.Build();

app.Services.RunMigrations();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.Run();

