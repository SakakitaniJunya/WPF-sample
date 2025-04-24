using Microsoft.AspNetCore.SignalR;
using TodoApp.Application.DTOs;
using TodoApp.Backend.WebSockets.Hubs;

namespace TodoApp.Backend.WebSockets;

public class TodoNotificationService
{
    private readonly IHubContext<TodoHub> _hubContext;
    
    public TodoNotificationService(IHubContext<TodoHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    public async Task NotifyTodoAdded(TodoItemDto todo)
    {
        await _hubContext.Clients.All.SendAsync("TodoAdded", todo);
    }
    
    public async Task NotifyTodoUpdated(TodoItemDto todo)
    {
        await _hubContext.Clients.All.SendAsync("TodoUpdated", todo);
    }
    
    public async Task NotifyTodoDeleted(Guid id)
    {
        await _hubContext.Clients.All.SendAsync("TodoDeleted", id);
    }
}
