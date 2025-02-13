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

            var results = await ProcessMessages();

            await UpdateSentMessages(results);
            await UpdateFailedMessages(results);

            await unitOfWork.CommitAsync();
        }
        catch (Exception)
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }

    private async Task<(Guid Id, bool IsAck)[]> ProcessMessages()
    {
        var outboxMessages = await repository.FetchMessagesByStatusAsync(
            config.BatchSize, MessageStatus.Pending, unitOfWork.Connection, unitOfWork.Transaction!);

        var messages = outboxMessages.ToList();
        if (messages.Count <= 0)
        {
            return [];
        }

        var produceTasks = messages.Select(async msg =>
        {
            var kafkaMessage = CreateKafkaMessage(msg!);
            var deliveryReport = await producer.ProduceAsync("order-outbox-service", kafkaMessage);

            return (msg!.Id, IsAck: deliveryReport.Status == PersistenceStatus.Persisted);
        });

        return await Task.WhenAll(produceTasks);
    }

    private async Task UpdateFailedMessages((Guid Id, bool IsAck)[] results)
    {
        var failedIds = results.Where(r => r.IsAck is false).Select(x => x.Id).ToList();
        if (failedIds.Count > 0)
        {
            await repository.UpdateAsync(failedIds, MessageStatus.Failed, unitOfWork.Connection, unitOfWork.Transaction!);
        }
    }

    private async Task UpdateSentMessages((Guid Id, bool IsAck)[] results)
    {
        var processedIds = results.Where(r => r.IsAck).Select(x => x.Id).ToList();
        if (processedIds.Count > 0)
        {
            await repository.UpdateAsync(processedIds, MessageStatus.Processed, unitOfWork.Connection, unitOfWork.Transaction!);
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