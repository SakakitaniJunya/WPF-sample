using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using TodoApp.ApiClients.Abstractions;
using TodoApp.Application.DTOs;

namespace TodoApp.ApiClients.RestClient;

public class OpenApiTodoClient : ITodoApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    
    public OpenApiTodoClient(HttpClient httpClient, string baseUrl)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl.TrimEnd('/');
    }
    
    public async Task<IEnumerable<TodoItemDto>> GetAllTodosAsync()
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/todos");
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<IEnumerable<TodoItemDto>>() ?? new List<TodoItemDto>();
    }
    
    public async Task<TodoItemDto?> GetTodoByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/todos/{id}");
        
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoItemDto>();
    }
    
    public async Task<TodoItemDto> CreateTodoAsync(string title)
    {
        var request = new { Title = title };
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync($"{_baseUrl}/api/todos", content);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<TodoItemDto>() ?? 
            throw new InvalidOperationException("Failed to deserialize response");
    }
    
    public async Task<TodoItemDto?> UpdateTodoAsync(Guid id, string title, bool isDone)
    {
        var request = new { Title = title, IsDone = isDone };
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PutAsync($"{_baseUrl}/api/todos/{id}", content);
        
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoItemDto>();
    }
    
    public async Task<bool> DeleteTodoAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/todos/{id}");
        
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
        
        response.EnsureSuccessStatusCode();
        return true;
    }
}
