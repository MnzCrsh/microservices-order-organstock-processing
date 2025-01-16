using Confluent.Kafka;
using OrderService.CQRS;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var kafkaConfig = builder.Configuration.GetSection("Kafka");
var redisConfig = builder.Configuration.GetSection("Redis");

builder.Services.AddOpenApi();

builder.Services.AddScoped<IOrderCommandProcessor, OrderCommandProcessor>();

builder.Services.AddSingleton<IProducer<string, string>>(_ =>
{
    var producerConfig = new ProducerConfig
    {
        BootstrapServers = kafkaConfig["BootstrapServers"],
        Acks = Enum.Parse<Acks>(kafkaConfig["Produce:Acks"] ?? Acks.All.ToString()),
        BatchSize = int.Parse(kafkaConfig["Produce:BatchSize"] ?? "16384"),
        LingerMs = int.Parse(kafkaConfig["Produce:LingerMs"] ?? "5"),
    };
    return new ProducerBuilder<string, string>(producerConfig).Build();
});

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig["ConnectionString"]!));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.Run();

