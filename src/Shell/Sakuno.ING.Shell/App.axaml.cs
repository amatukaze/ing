using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sakuno.ING.ViewModels.Bootstrap;
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
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return;

        var host = AppLocator.Current.GetService<IHost>()!;
        var container = host.Services.GetRequiredService<IContainer>();

        desktop.Exit += (sender, args) =>
        {
            host.StopAsync().GetAwaiter().GetResult();
        };

        var bootstrapViewModel = host.Services.GetRequiredService<BootstrapViewModel>();

        var bootstrapWindow = new BootstrapWindow
        {
            DataContext = bootstrapViewModel,
        };

        var bootstrapCompleted = false;
        bootstrapWindow.Closed += (sender, args) =>
        {
            if (!bootstrapCompleted)
                desktop.Shutdown();
        };

        desktop.MainWindow = bootstrapWindow;

        bootstrapViewModel.Start();

        _ = EnterMainUiAsync();

        async Task EnterMainUiAsync()
        {
            try
            {
                await bootstrapViewModel.Completion;
            }
            catch (OperationCanceledException)
            {
                bootstrapWindow.Close();
                return;
            }

            bootstrapCompleted = true;

            desktop.MainWindow = new MainWindow();

            var browserWindow = new GameBrowserWindow();
            browserWindow.Show();

            DependencyInjection.SetContainer(desktop.MainWindow, container);

            bootstrapWindow.Close();
        }
    }
}
