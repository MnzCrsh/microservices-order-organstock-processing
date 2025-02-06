using System.Data;
using System.Text;
using Confluent.Kafka;
using OrderService.Application.Abstractions;
using OrderService.Entities;
using OrderService.Entities.Models.Entities;
using OrderService.Entities.Models.Responses;
using OrderService.Mapping;
using OrderService.Repositories.Abstractions;

namespace OrderService.Application;

/// <inheritdoc/>
public class OutboxService(
    IOutboxRepository repository,
    IUnitOfWork unitOfWork,
    IProducer<Guid, OutboxResponseModel> producer,
    IMapperFactory mapperFactory,
    OutboxConfig config) : IOutboxService
{
    /// <inheritdoc/>
    public async Task ProcessAsync()
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();

            var outboxMessages = await repository.FetchMessagesByStatusAsync(
                config.BatchSize, MessageStatus.Pending, unitOfWork.Connection, unitOfWork.Transaction);

            var messages = outboxMessages.ToList();
            if (messages.Count <= 0)
            {
                return;
            }

            var produceTasks = messages.Select(async msg =>
            {
                var kafkaMessage = CreateKafkaMessage(msg!);
                var deliveryReport = await producer.ProduceAsync("order-outbox-service", kafkaMessage);

                return (msg!.Id, IsAck: deliveryReport.Status == PersistenceStatus.Persisted);
            });

            var results = Task.WhenAll(produceTasks);

            await UpdateSentMessages(results, unitOfWork.Connection, unitOfWork.Transaction);
            await UpdateFailedMessages(results, unitOfWork.Connection, unitOfWork.Transaction);

            await unitOfWork.CommitAsync();
        }
        catch (Exception)
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }

    private async Task UpdateFailedMessages(Task<(Guid Id, bool IsAck)[]> results, IDbConnection conn, IDbTransaction transaction)
    {
        var failedIds = results.Result.Where(r => r.IsAck is false).Select(x => x.Id).ToList();
        if (failedIds.Count > 0)
        {
            await repository.UpdateAsync(failedIds, MessageStatus.Failed, conn, transaction);
        }
    }

    private async Task UpdateSentMessages(Task<(Guid Id, bool IsAck)[]> results, IDbConnection conn, IDbTransaction transaction)
    {
        var processedIds = results.Result.Where(r => r.IsAck).Select(x => x.Id).ToList();
        if (processedIds.Count > 0)
        {
            await repository.UpdateAsync(processedIds, MessageStatus.Processed, conn, transaction);
        }
    }

    private Message<Guid, OutboxResponseModel> CreateKafkaMessage(OutboxMessage msg)
    {
        var mapper = mapperFactory.GetMapper<OutboxMessage, OutboxResponseModel>();

        var outboxResponseMessage = mapper.Map(msg);
        var kafkaMessage = new Message<Guid, OutboxResponseModel>
        {
            Key = outboxResponseMessage.Id,
            Value = outboxResponseMessage,
            Headers = new Headers
            {
                { "EventType", Encoding.UTF8.GetBytes(outboxResponseMessage.EventType) }
            }
        };
        return kafkaMessage;
    }
}