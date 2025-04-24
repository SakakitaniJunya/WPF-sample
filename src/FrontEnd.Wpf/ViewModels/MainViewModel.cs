using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TodoApp.ApiClients.ClientCore;
using TodoApp.Application.DTOs;

namespace TodoApp.FrontEnd.Wpf.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly TodoApiGateway _apiGateway;
    private string _newTodoTitle = string.Empty;
    private string _connectionStatus = "未接続";
    private bool _isConnected;
    private bool _isLoading;
    
    public ObservableCollection<TodoItemViewModel> TodoItems { get; } = new();
    
    public string NewTodoTitle
    {
        get => _newTodoTitle;
        set => SetProperty(ref _newTodoTitle, value);
    }
    
    public string ConnectionStatus
    {
        get => _connectionStatus;
        set => SetProperty(ref _connectionStatus, value);
    }
    
    public bool IsConnected
    {
        get => _isConnected;
        set => SetProperty(ref _isConnected, value);
    }
    
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    
    public ICommand AddTodoCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand ConnectCommand { get; }
    public ICommand DisconnectCommand { get; }
    
    public MainViewModel(TodoApiGateway apiGateway)
    {
        _apiGateway = apiGateway;
        
        AddTodoCommand = new AsyncRelayCommand(AddTodoAsync, () => !string.IsNullOrWhiteSpace(NewTodoTitle) && IsConnected);
        RefreshCommand = new AsyncRelayCommand(RefreshTodosAsync);
        ConnectCommand = new AsyncRelayCommand(ConnectAsync, () => !IsConnected);
        DisconnectCommand = new AsyncRelayCommand(DisconnectAsync, () => IsConnected);
        
        // イベントハンドラの登録
        _apiGateway.TodoAdded += OnTodoAdded;
        _apiGateway.TodoUpdated += OnTodoUpdated;
        _apiGateway.TodoDeleted += OnTodoDeleted;
    }
    
    public async Task InitializeAsync()
    {
        await ConnectAsync();
        await RefreshTodosAsync();
    }
    
    private async Task ConnectAsync()
    {
        try
        {
            IsLoading = true;
            ConnectionStatus = "接続中...";
            
            await _apiGateway.ConnectAsync();
            
            ConnectionStatus = "接続済み";
            IsConnected = true;
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"接続エラー: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private async Task DisconnectAsync()
    {
        try
        {
            IsLoading = true;
            ConnectionStatus = "切断中...";
            
            await _apiGateway.DisconnectAsync();
            
            ConnectionStatus = "未接続";
            IsConnected = false;
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"切断エラー: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private async Task RefreshTodosAsync()
    {
        if (!IsConnected)
        {
            return;
        }
        
        try
        {
            IsLoading = true;
            var todos = await _apiGateway.GetAllTodosAsync();
            
            TodoItems.Clear();
            
            foreach (var todo in todos)
            {
                TodoItems.Add(CreateTodoViewModel(todo));
            }
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"データ取得エラー: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private async Task AddTodoAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTodoTitle) || !IsConnected)
        {
            return;
        }
        
        try
        {
            IsLoading = true;
            await _apiGateway.CreateTodoAsync(NewTodoTitle);
            NewTodoTitle = string.Empty;
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Todo追加エラー: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private async Task UpdateTodoAsync(Guid id, string title, bool isDone)
    {
        if (!IsConnected)
        {
            return;
        }
        
        try
        {
            IsLoading = true;
            await _apiGateway.UpdateTodoAsync(id, title, isDone);
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Todo更新エラー: {ex.Message}";
            await RefreshTodosAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private async Task DeleteTodoAsync(Guid id)
    {
        if (!IsConnected)
        {
            return;
        }
        
        try
        {
            IsLoading = true;
            await _apiGateway.DeleteTodoAsync(id);
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Todo削除エラー: {ex.Message}";
            await RefreshTodosAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private void OnTodoAdded(TodoItemDto todo)
    {
        // UI スレッドで実行
        App.Current.Dispatcher.Invoke(() =>
        {
            var todoVm = CreateTodoViewModel(todo);
            TodoItems.Add(todoVm);
        });
    }
    
    private void OnTodoUpdated(TodoItemDto todo)
    {
        // UI スレッドで実行
        App.Current.Dispatcher.Invoke(() =>
        {
            var existingTodo = TodoItems.FirstOrDefault(t => t.Id == todo.Id);
            
            if (existingTodo != null)
            {
                existingTodo.Update(todo);
            }
            else
            {
                // 存在しない場合は追加
                var todoVm = CreateTodoViewModel(todo);
                TodoItems.Add(todoVm);
            }
        });
    }
    
    private void OnTodoDeleted(Guid id)
    {
        // UI スレッドで実行
        App.Current.Dispatcher.Invoke(() =>
        {
            var todoToRemove = TodoItems.FirstOrDefault(t => t.Id == id);
            
            if (todoToRemove != null)
            {
                TodoItems.Remove(todoToRemove);
            }
        });
    }
    
    private TodoItemViewModel CreateTodoViewModel(TodoItemDto todo)
    {
        return new TodoItemViewModel(todo, UpdateTodoAsync, DeleteTodoAsync);
    }
}
