using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using System.Windows.Threading;

namespace Sakuno.ING.Shell.Hosting;

internal class ShellHostedService(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime) : IHostedService
{
    private Application _app = default!;

    private bool _isRunning;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var uiThread = new Thread(UIThreadStart) { IsBackground = true };

        uiThread.SetApartmentState(ApartmentState.STA);
        uiThread.Start();

        return Task.CompletedTask;
    }

    private void UIThreadStart()
    {
        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));

        _app = serviceProvider.GetService<Application>() ?? new Application()
        {
            ShutdownMode = ShutdownMode.OnMainWindowClose,
        };
        _app.Startup += delegate
        {
            _isRunning = true;
        };
        _app.Exit += delegate
        {
            _isRunning = false;

            hostApplicationLifetime.StopApplication();
        };

        var mainWindow = serviceProvider.GetRequiredService<IShellMainWindow>() as Window ??
            throw new InvalidOperationException("No main window is provided");

        mainWindow.Show();

        _app.MainWindow = mainWindow;

        _app.Run();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (!_isRunning)
            return;

        await _app.Dispatcher.InvokeAsync(_app.Shutdown);
    }
}
