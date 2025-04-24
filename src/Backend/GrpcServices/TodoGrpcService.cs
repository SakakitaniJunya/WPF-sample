using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using TodoApp.Application.Queries;

namespace TodoApp.Backend.GrpcServices;

public class TodoGrpcService : TodoService.TodoServiceBase
{
    private readonly IMediator _mediator;
    
    public TodoGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public override async Task StreamTodos(Empty request, IServerStreamWriter<TodoDto> responseStream, ServerCallContext context)
    {
        var todos = await _mediator.Send(new GetAllTodosQuery());
        
        foreach (var todo in todos)
        {
            if (context.CancellationToken.IsCancellationRequested)
                break;
                
            var todoDto = new TodoDto
            {
                Id = todo.Id.ToString(),
                Title = todo.Title,
                IsDone = todo.IsDone,
                CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(todo.CreatedAt, DateTimeKind.Utc)),
                UpdatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(todo.UpdatedAt, DateTimeKind.Utc))
            };
            
            await responseStream.WriteAsync(todoDto);
        }
    }
}
