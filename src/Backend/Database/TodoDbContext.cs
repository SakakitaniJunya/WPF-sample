using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Todo;

namespace TodoApp.Backend.Database;

public class TodoDbContext : DbContext
{
    public DbSet<TodoItem> Todos { get; set; } = null!;
    
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>()
            .HasKey(t => t.Id);
            
        modelBuilder.Entity<TodoItem>()
            .Property(t => t.Title)
            .IsRequired();
    }
}
