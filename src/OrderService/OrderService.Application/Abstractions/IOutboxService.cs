namespace OrderService.Application.Abstractions;

/// <summary>
/// Service that processes OutBox messages
/// </summary>
public interface IOutboxService
{
    /// <summary>
    /// Processes outbox messages
    /// </summary>
    public Task ProcessAsync();
}