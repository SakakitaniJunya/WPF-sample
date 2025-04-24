using TodoApp.Application.DTOs;

namespace TodoApp.ApiClients.Abstractions;

/// <summary>
/// Todo REST API クライアントインターフェース
/// </summary>
public interface ITodoApiClient
{
    Task<IEnumerable<TodoItemDto>> GetAllTodosAsync();
    Task<TodoItemDto?> GetTodoByIdAsync(Guid id);
    Task<TodoItemDto> CreateTodoAsync(string title);
    Task<TodoItemDto?> UpdateTodoAsync(Guid id, string title, bool isDone);
    Task<bool> DeleteTodoAsync(Guid id);
}
