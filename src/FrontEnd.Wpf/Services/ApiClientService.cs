using TodoApp.ApiClients.Abstractions;
using TodoApp.ApiClients.ClientCore;
using TodoApp.ApiClients.GrpcClient;
using TodoApp.ApiClients.RestClient;
using TodoApp.ApiClients.WebSocketClient;

namespace TodoApp.FrontEnd.Wpf.Services;

public class ApiClientService
{
    private readonly string _serverUrl;
    private readonly HttpClient _httpClient;
    
    public TodoApiGateway TodoGateway { get; }
    
    public ApiClientService(string serverUrl)
    {
        _serverUrl = serverUrl;
        _httpClient = new HttpClient();
        
        // 各APIクライアントを作成
        ITodoApiClient restClient = new OpenApiTodoClient(_httpClient, serverUrl);
        ITodoStreamClient streamClient = new GrpcTodoClient(serverUrl);
        ITodoNotificationClient notificationClient = new SignalRTodoClient(serverUrl);
        
        // ゲートウェイ作成
        TodoGateway = new TodoApiGateway(restClient, streamClient, notificationClient);
    }
}
