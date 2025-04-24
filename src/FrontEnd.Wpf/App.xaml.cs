using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using TodoApp.FrontEnd.Wpf.Services;
using TodoApp.FrontEnd.Wpf.ViewModels;
using TodoApp.FrontEnd.Wpf.Views;

namespace TodoApp.FrontEnd.Wpf;

public partial class App : Application
{
    private readonly IHost _host;
    
    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(ConfigureServices)
            .Build();
    }
    
    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        // 設定
        string serverUrl = "http://localhost:5000";
        
        // サービス登録
        services.AddSingleton(new ApiClientService(serverUrl));
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();
    }
    
    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();
        
        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
        
        base.OnStartup(e);
    }
    
    protected override async void OnExit(ExitEventArgs e)
    {
        using (_host)
        {
            await _host.StopAsync(TimeSpan.FromSeconds(5));
        }
        
        base.OnExit(e);
    }
}
