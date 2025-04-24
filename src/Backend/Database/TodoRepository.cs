using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Todo;

namespace TodoApp.Backend.Database;

public class TodoRepository : ITodoRepository
{
    private readonly TodoDbContext _context;
    
    public TodoRepository(TodoDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
        return await _context.Todos.ToListAsync();
    }
    
    public async Task<TodoItem?> GetByIdAsync(Guid id)
    {
        return await _context.Todos.FindAsync(id);
    }
    
    public async Task<TodoItem> AddAsync(TodoItem todo)
    {
        _context.Todos.Add(todo);
        return todo;
    }
    
    public async Task<TodoItem> UpdateAsync(TodoItem todo)
    {
        _context.Entry(todo).State = EntityState.Modified;
        return todo;
    }
    
    public async Task DeleteAsync(Guid id)
    {
        var todo = await GetByIdAsync(id);
        if (todo != null)
        {
            _context.Todos.Remove(todo);
        }
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
