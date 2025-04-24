using MediatR;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Todo;

namespace TodoApp.Application.Queries;

/// <summary>
/// 全Todoアイテム取得クエリ
/// </summary>
public record GetAllTodosQuery() : IRequest<IEnumerable<TodoItemDto>>;

public class GetAllTodosQueryHandler : IRequestHandler<GetAllTodosQuery, IEnumerable<TodoItemDto>>
{
    private readonly ITodoRepository _repository;

    public GetAllTodosQueryHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TodoItemDto>> Handle(GetAllTodosQuery request, CancellationToken cancellationToken)
    {
        var todos = await _repository.GetAllAsync();
        
        return todos.Select(todo => new TodoItemDto
        {
            Id = todo.Id,
            Title = todo.Title,
            IsDone = todo.IsDone,
            CreatedAt = todo.CreatedAt,
            UpdatedAt = todo.UpdatedAt
        });
    }
}
