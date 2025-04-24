using System.Diagnostics;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace TodoApp.Tools.OpenApiGenerator;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("OpenAPI Generator Tool Starting...");
        
        // バックエンドサーバーのURL
        string serverUrl = "http://localhost:5000";
        
        // ターゲットディレクトリ
        string targetDirectory = "../../src/ApiClients/RestClient/Generated";
        
        try
        {
            // Swagger JSONの取得
            Console.WriteLine("Fetching Swagger JSON from server...");
            string? swaggerJson = await GetSwaggerJsonAsync(serverUrl);
            
            if (string.IsNullOrEmpty(swaggerJson))
            {
                Console.WriteLine("Failed to get Swagger JSON. Is the server running?");
                return;
            }
            
            // OpenAPI Generator実行
            Console.WriteLine("Running OpenAPI Generator...");
            await RunOpenApiGeneratorAsync(swaggerJson, targetDirectory);
            
            Console.WriteLine("OpenAPI Generator completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
    
    private static async Task<string?> GetSwaggerJsonAsync(string serverUrl)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync($"{serverUrl.TrimEnd('/')}/swagger/v1/swagger.json");
        
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        
        return null;
    }
    
    private static async Task RunOpenApiGeneratorAsync(string swaggerJson, string targetDirectory)
    {
        // 一時ファイルにSwagger JSONを保存
        string tempFilePath = Path.GetTempFileName();
        await File.WriteAllTextAsync(tempFilePath, swaggerJson);
        
        // ディレクトリが存在することを確認
        Directory.CreateDirectory(targetDirectory);
        
        // OpenAPI Generator CLIコマンド
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "openapi-generator-cli",
            Arguments = $"generate -i {tempFilePath} -g csharp -o {targetDirectory} --additional-properties=netCoreProjectFile=true,packageName=TodoApp.ApiClients.RestClient.Generated",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        // プロセス実行
        using var process = new Process();
        process.StartInfo = psi;
        
        process.OutputDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
        process.ErrorDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine($"ERROR: {e.Data}"); };
        
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        
        await process.WaitForExitAsync();
        
        // 一時ファイル削除
        File.Delete(tempFilePath);
        
        if (process.ExitCode != 0)
        {
            throw new Exception($"OpenAPI Generator failed with exit code {process.ExitCode}");
        }
    }
}
