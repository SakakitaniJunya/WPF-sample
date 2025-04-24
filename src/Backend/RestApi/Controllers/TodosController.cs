using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Commands;
using TodoApp.Application.DTOs;
using TodoApp.Application.Queries;

namespace TodoApp.Backend.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public TodosController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var todos = await _mediator.Send(new GetAllTodosQuery());
        return Ok(todos);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var todo = await _mediator.Send(new GetTodoByIdQuery(id));
        
        if (todo == null)
        {
            return NotFound();
        }
        
        return Ok(todo);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTodoRequest request)
    {
        var todo = await _mediator.Send(new CreateTodoCommand(request.Title));
        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTodoRequest request)
    {
        var todo = await _mediator.Send(new UpdateTodoCommand(id, request.Title, request.IsDone));
        
        if (todo == null)
        {
            return NotFound();
        }
        
        return Ok(todo);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _mediator.Send(new DeleteTodoCommand(id));
        
        if (!success)
        {
            return NotFound();
        }
        
        return NoContent();
    }
}

public record CreateTodoRequest(string Title);
public record UpdateTodoRequest(string Title, bool IsDone);
