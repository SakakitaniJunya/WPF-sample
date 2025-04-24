using TodoApp.ApiClients.Abstractions;
using TodoApp.Application.DTOs;

namespace TodoApp.ApiClients.ClientCore;

/// <summary>
/// TodoAPI統合ゲートウェイ
/// </summary>
public class TodoApiGateway
{
    private readonly ITodoApiClient _restClient;
    private readonly ITodoStreamClient _streamClient;
    private readonly ITodoNotificationClient _notificationClient;
    
    public event Action<TodoItemDto>? TodoAdded;
    public event Action<TodoItemDto>? TodoUpdated;
    public event Action<Guid>? TodoDeleted;
    
    public TodoApiGateway(
        ITodoApiClient restClient, 
        ITodoStreamClient streamClient, 
        ITodoNotificationClient notificationClient)
    {
        _restClient = restClient;
        _streamClient = streamClient;
        _notificationClient = notificationClient;
        
        // リレーイベント
        _notificationClient.TodoAdded += todo => TodoAdded?.Invoke(todo);
        _notificationClient.TodoUpdated += todo => TodoUpdated?.Invoke(todo);
        _notificationClient.TodoDeleted += id => TodoDeleted?.Invoke(id);
    }
    
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        await _notificationClient.ConnectAsync(cancellationToken);
    }
    
    public async Task DisconnectAsync()
    {
        await _notificationClient.DisconnectAsync();
    }
    
    public async Task<IEnumerable<TodoItemDto>> GetAllTodosAsync()
    {
        return await _restClient.GetAllTodosAsync();
    }
    
    public async Task<TodoItemDto?> GetTodoByIdAsync(Guid id)
    {
        return await _restClient.GetTodoByIdAsync(id);
    }
    
    public async Task<TodoItemDto> CreateTodoAsync(string title)
    {
        return await _restClient.CreateTodoAsync(title);
    }
    
    public async Task<TodoItemDto?> UpdateTodoAsync(Guid id, string title, bool isDone)
    {
        return await _restClient.UpdateTodoAsync(id, title, isDone);
    }
    
    public async Task<bool> DeleteTodoAsync(Guid id)
    {
        return await _restClient.DeleteTodoAsync(id);
    }
    
    public IAsyncEnumerable<TodoItemDto> StreamTodosAsync(CancellationToken cancellationToken = default)
    {
        return _streamClient.StreamTodosAsync(cancellationToken);
    }
}
