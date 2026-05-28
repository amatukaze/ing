using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Splat;

namespace Sakuno.ING.Shell;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var host = AppLocator.Current.GetService<IHost>()!;

        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return;

        desktop.Startup += (sender, args) =>
        {
            Task.Factory.StartNew(host.Start, TaskCreationOptions.LongRunning).ContinueWith(t =>
            {
                host.Services.GetRequiredService<ILogger<App>>().LogError(t.Exception, "Unhandled exception from host thread");
            }, TaskContinuationOptions.OnlyOnFaulted);
        };
        desktop.Exit += (sender, e) =>
        {
            host.StopAsync().GetAwaiter().GetResult();
        };

        desktop.MainWindow = new MainWindow();

        DependencyInjection.SetContainer(desktop.MainWindow, host.Services.GetRequiredService<IContainer>());
    }
}
