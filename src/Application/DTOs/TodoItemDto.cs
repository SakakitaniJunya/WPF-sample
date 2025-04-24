namespace TodoApp.Application.DTOs;

/// <summary>
/// Todo項目データ転送オブジェクト
/// </summary>
public class TodoItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
