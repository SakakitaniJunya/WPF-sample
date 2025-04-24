using TodoApp.Application.DTOs;

namespace TodoApp.ApiClients.Abstractions;

/// <summary>
/// Todo gRPCストリーミングクライアントインターフェース
/// </summary>
public interface ITodoStreamClient
{
    /// <summary>
    /// Todoアイテムをストリーミング取得します
    /// </summary>
    IAsyncEnumerable<TodoItemDto> StreamTodosAsync(CancellationToken cancellationToken = default);
}
