using Microsoft.AspNetCore.SignalR.Client;
using TodoApp.ApiClients.Abstractions;
using TodoApp.Application.DTOs;

namespace TodoApp.ApiClients.WebSocketClient;

public class SignalRTodoClient : ITodoNotificationClient, IAsyncDisposable
{
    private readonly string _hubUrl;
    private HubConnection? _hubConnection;
    
    public event Action<TodoItemDto>? TodoAdded;
    public event Action<TodoItemDto>? TodoUpdated;
    public event Action<Guid>? TodoDeleted;
    
    public SignalRTodoClient(string serverUrl)
    {
        _hubUrl = $"{serverUrl.TrimEnd('/')}/todoHub";
    }
    
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        if (_hubConnection != null)
        {
            return;
        }
        
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl)
            .WithAutomaticReconnect()
            .Build();
            
        // Add event handlers
        _hubConnection.On<TodoItemDto>("TodoAdded", todo => TodoAdded?.Invoke(todo));
        _hubConnection.On<TodoItemDto>("TodoUpdated", todo => TodoUpdated?.Invoke(todo));
        _hubConnection.On<Guid>("TodoDeleted", id => TodoDeleted?.Invoke(id));
        
        await _hubConnection.StartAsync(cancellationToken);
    }
    
    public async Task DisconnectAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }
}
