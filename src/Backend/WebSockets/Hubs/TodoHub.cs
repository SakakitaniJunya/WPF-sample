using Microsoft.AspNetCore.SignalR;
using TodoApp.Application.DTOs;

namespace TodoApp.Backend.WebSockets.Hubs;

public class TodoHub : Hub
{
    public async Task NotifyTodoAdded(TodoItemDto todo)
    {
        await Clients.All.SendAsync("TodoAdded", todo);
    }
    
    public async Task NotifyTodoUpdated(TodoItemDto todo)
    {
        await Clients.All.SendAsync("TodoUpdated", todo);
    }
    
    public async Task NotifyTodoDeleted(Guid id)
    {
        await Clients.All.SendAsync("TodoDeleted", id);
    }
}
