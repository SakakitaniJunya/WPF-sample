using TodoApp.Application.DTOs;

namespace TodoApp.ApiClients.Abstractions;

/// <summary>
/// Todo SignalR通知クライアントインターフェース
/// </summary>
public interface ITodoNotificationClient
{
    event Action<TodoItemDto> TodoAdded;
    event Action<TodoItemDto> TodoUpdated;
    event Action<Guid> TodoDeleted;
    
    Task ConnectAsync(CancellationToken cancellationToken = default);
    Task DisconnectAsync();
}
