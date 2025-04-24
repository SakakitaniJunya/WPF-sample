using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using TodoApp.Application.DTOs;

namespace TodoApp.FrontEnd.Wpf.ViewModels;

public class TodoItemViewModel : ObservableObject
{
    private readonly TodoItemDto _todoItem;
    private readonly Func<Guid, string, bool, Task> _updateTodoAction;
    private readonly Func<Guid, Task> _deleteTodoAction;
    
    private string _editedTitle;
    private bool _isEditing;
    
    public Guid Id => _todoItem.Id;
    public DateTime CreatedAt => _todoItem.CreatedAt;
    public DateTime UpdatedAt => _todoItem.UpdatedAt;
    
    public string Title 
    { 
        get => _todoItem.Title;
        private set => SetProperty(_todoItem.Title, value, _todoItem, (todo, newTitle) => todo.Title = newTitle);
    }
    
    public bool IsDone 
    { 
        get => _todoItem.IsDone;
        set 
        {
            if (SetProperty(_todoItem.IsDone, value, _todoItem, (todo, newValue) => todo.IsDone = newValue))
            {
                _ = UpdateTodoAsync();
            }
        }
    }
    
    public string EditedTitle
    {
        get => _editedTitle;
        set => SetProperty(ref _editedTitle, value);
    }
    
    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }
    
    public ICommand StartEditCommand { get; }
    public ICommand SaveEditCommand { get; }
    public ICommand CancelEditCommand { get; }
    public ICommand DeleteCommand { get; }
    
    public TodoItemViewModel(
        TodoItemDto todoItem,
        Func<Guid, string, bool, Task> updateTodoAction,
        Func<Guid, Task> deleteTodoAction)
    {
        _todoItem = todoItem;
        _updateTodoAction = updateTodoAction;
        _deleteTodoAction = deleteTodoAction;
        _editedTitle = todoItem.Title;
        
        StartEditCommand = new RelayCommand(StartEdit);
        SaveEditCommand = new AsyncRelayCommand(SaveEditAsync);
        CancelEditCommand = new RelayCommand(CancelEdit);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync);
    }
    
    private void StartEdit()
    {
        EditedTitle = Title;
        IsEditing = true;
    }
    
    private async Task SaveEditAsync()
    {
        if (string.IsNullOrWhiteSpace(EditedTitle))
        {
            return;
        }
        
        Title = EditedTitle;
        IsEditing = false;
        await UpdateTodoAsync();
    }
    
    private void CancelEdit()
    {
        IsEditing = false;
        EditedTitle = Title;
    }
    
    private async Task UpdateTodoAsync()
    {
        await _updateTodoAction(Id, Title, IsDone);
    }
    
    private async Task DeleteAsync()
    {
        await _deleteTodoAction(Id);
    }
    
    public void Update(TodoItemDto updatedTodo)
    {
        if (updatedTodo.Id != Id)
        {
            return;
        }
        
        Title = updatedTodo.Title;
        _todoItem.IsDone = updatedTodo.IsDone;
        OnPropertyChanged(nameof(IsDone));
    }
}
