namespace TodoApp.Domain.Todo;

/// <summary>
/// TodoItem エンティティ
/// </summary>
public class TodoItem
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public bool IsDone { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private TodoItem() { }

    public TodoItem(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty", nameof(title));
        }

        Id = Guid.NewGuid();
        Title = title;
        IsDone = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void MarkAsDone()
    {
        IsDone = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsUndone()
    {
        IsDone = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
        {
            throw new ArgumentException("Title cannot be empty", nameof(newTitle));
        }

        Title = newTitle;
        UpdatedAt = DateTime.UtcNow;
    }
}
