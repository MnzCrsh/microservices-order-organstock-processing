using OrderService.Core;
using OrderService.CQRS;
using OrderService.Postgres;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var kafkaConfig = builder.Configuration.GetSection("Kafka");
var postgresConfig = builder.Configuration.GetSection("Postgres");

builder.Services
    .AddOpenApi()
    .AddMigrations(postgresConfig["ConnectionString"])
    .AddKafkaProducers(kafkaConfig)
    .AddRedis(builder.Configuration);

builder.Services
    .AddScoped<IOrderCommandProcessor, OrderCommandProcessor>();

var app = builder.Build();

app.Services.RunMigrations();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.Run();

