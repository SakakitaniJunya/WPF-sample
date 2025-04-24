namespace TodoApp.Domain.Todo;

/// <summary>
/// Todo リポジトリインターフェース
/// </summary>
public interface ITodoRepository
{
    Task<IEnumerable<TodoItem>> GetAllAsync();
    Task<TodoItem?> GetByIdAsync(Guid id);
    Task<TodoItem> AddAsync(TodoItem todo);
    Task<TodoItem> UpdateAsync(TodoItem todo);
    Task DeleteAsync(Guid id);
    Task<int> SaveChangesAsync();
}
