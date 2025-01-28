using OrderService.Entities.Models;
using OrderService.Entities.Models.Responses;

namespace OrderService.Repositories;

public interface IOutboxRepository
{
    Task<OutboxResponseModel> SaveAsync(OutboxMessage outboxMessage);
    
    Task<OutboxResponseModel> UpdateAsync(OutboxMessage outboxMessage);
    
    Task<IEnumerable<OutboxResponseModel>?> FetchUnprocessedMessagesAsync();    
}