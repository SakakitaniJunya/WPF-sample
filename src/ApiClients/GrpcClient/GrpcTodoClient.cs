using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using TodoApp.ApiClients.Abstractions;
using TodoApp.Application.DTOs;
using TodoApp.Backend.GrpcServices;

namespace TodoApp.ApiClients.GrpcClient;

public class GrpcTodoClient : ITodoStreamClient
{
    private readonly string _serverAddress;
    
    public GrpcTodoClient(string serverAddress)
    {
        _serverAddress = serverAddress;
    }
    
    public async IAsyncEnumerable<TodoItemDto> StreamTodosAsync(CancellationToken cancellationToken = default)
    {
        using var channel = GrpcChannel.ForAddress(_serverAddress);
        var client = new TodoService.TodoServiceClient(channel);
        
        using var streamingCall = client.StreamTodos(new Empty(), cancellationToken: cancellationToken);
        
        await foreach (var todoDto in streamingCall.ResponseStream.ReadAllAsync(cancellationToken))
        {
            yield return ConvertToDto(todoDto);
        }
    }
    
    private static TodoItemDto ConvertToDto(TodoDto grpcDto)
    {
        return new TodoItemDto
        {
            Id = Guid.Parse(grpcDto.Id),
            Title = grpcDto.Title,
            IsDone = grpcDto.IsDone,
            CreatedAt = grpcDto.CreatedAt.ToDateTime(),
            UpdatedAt = grpcDto.UpdatedAt.ToDateTime()
        };
    }
}
