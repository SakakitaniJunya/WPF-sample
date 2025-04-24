using MediatR;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Todo;

namespace TodoApp.Application.Queries;

/// <summary>
/// ID指定でTodoアイテム取得クエリ
/// </summary>
public record GetTodoByIdQuery(Guid Id) : IRequest<TodoItemDto?>;

public class GetTodoByIdQueryHandler : IRequestHandler<GetTodoByIdQuery, TodoItemDto?>
{
    private readonly ITodoRepository _repository;

    public GetTodoByIdQueryHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<TodoItemDto?> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
    {
        var todo = await _repository.GetByIdAsync(request.Id);
        
        if (todo == null)
        {
            return null;
        }
        
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
