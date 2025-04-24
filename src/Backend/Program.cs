using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Commands;
using TodoApp.Backend.Database;
using TodoApp.Backend.GrpcServices;
using TodoApp.Backend.WebSockets;
using TodoApp.Backend.WebSockets.Hubs;
using TodoApp.Domain.Todo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR();

// Add DbContext
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseInMemoryDatabase("TodoDb"));

// Add Repository
builder.Services.AddScoped<ITodoRepository, TodoRepository>();

// Add MediatR
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(CreateTodoCommand).Assembly));

// Add gRPC
builder.Services.AddGrpc();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
    });
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Notification Service
builder.Services.AddSingleton<TodoNotificationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<TodoGrpcService>().RequireCors("AllowAll");
app.MapHub<TodoHub>("/todoHub");

app.Run();
