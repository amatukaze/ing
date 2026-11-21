using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;

namespace Sakuno.ING.Shell.UIComponentAutomation;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var builder = Host.CreateApplicationBuilder(args);
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);
        builder.Logging.SetMinimumLevel(LogLevel.Warning);

        builder.Services.AddSingleton<UiComponentScenarioCatalog>();
        builder.Services.AddSingleton<UiComponentSessionCoordinator>();
        builder.Services.AddSingleton<UiComponentAutomationTools>();
        builder.Services
            .AddMcpServer(options => options.ServerInfo = new()
            {
                Name = "ing-ui-component-automation",
                Version = "1.0.0",
            })
            .WithStdioServerTransport()
            .WithTools<UiComponentAutomationTools>();

        try
        {
            await builder.Build().RunAsync();
            return 0;
        }
        catch (Exception exception)
        {
            await Console.Error.WriteLineAsync($"UI Component Automation host failed: {exception}");
            return 1;
        }
    }
}
