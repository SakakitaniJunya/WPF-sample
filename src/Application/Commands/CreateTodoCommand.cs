using MediatR;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Todo;

namespace TodoApp.Application.Commands;

/// <summary>
/// 新規Todoアイテム作成コマンド
/// </summary>
public record CreateTodoCommand(string Title) : IRequest<TodoItemDto>;

public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, TodoItemDto>
{
    private readonly ITodoRepository _repository;

    public CreateTodoCommandHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<TodoItemDto> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = new TodoItem(request.Title);
        
        await _repository.AddAsync(todo);
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
