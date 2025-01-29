using System.Data;
using OrderService.Entities.Models;
using OrderService.Entities.Models.Responses;

namespace OrderService.Repositories.Abstractions;

public interface IOutboxRepository
{
    Task<OutboxResponseModel> SaveAsync(OutboxMessage outboxMessage, IDbConnection connection, IDbTransaction transaction);
    
    Task<bool> UpdateAsync(OutboxMessage outboxMessage, IDbConnection connection, IDbTransaction transaction);
    
    Task<IEnumerable<OutboxResponseModel>?> FetchUnprocessedMessagesAsync(IDbConnection connection, IDbTransaction transaction);    
}