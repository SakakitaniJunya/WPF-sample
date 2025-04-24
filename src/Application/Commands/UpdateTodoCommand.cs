using MediatR;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Todo;

namespace TodoApp.Application.Commands;

/// <summary>
/// Todoアイテム更新コマンド
/// </summary>
public record UpdateTodoCommand(Guid Id, string Title, bool IsDone) : IRequest<TodoItemDto?>;

public class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand, TodoItemDto?>
{
    private readonly ITodoRepository _repository;

    public UpdateTodoCommandHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<TodoItemDto?> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _repository.GetByIdAsync(request.Id);
        
        if (todo == null)
        {
            return null;
        }
        
        todo.UpdateTitle(request.Title);
        
        if (request.IsDone)
        {
            todo.MarkAsDone();
        }
        else
        {
            todo.MarkAsUndone();
        }
        
        await _repository.UpdateAsync(todo);
        await _repository.SaveChangesAsync();
        
        return new TodoItemDto
        {
            Id = todo.Id,
            Title = todo.Title,
            IsDone = todo.IsDone,
            CreatedAt = todo.CreatedAt,
            UpdatedAt = todo.UpdatedAt
        };
    }
}
