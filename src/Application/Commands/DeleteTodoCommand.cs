using MediatR;
using TodoApp.Domain.Todo;

namespace TodoApp.Application.Commands;

/// <summary>
/// Todoアイテム削除コマンド
/// </summary>
public record DeleteTodoCommand(Guid Id) : IRequest<bool>;

public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand, bool>
{
    private readonly ITodoRepository _repository;

    public DeleteTodoCommandHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _repository.GetByIdAsync(request.Id);
        
        if (todo == null)
        {
            return false;
        }
        
        await _repository.DeleteAsync(request.Id);
        await _repository.SaveChangesAsync();
        
        return true;
    }
}
