using Avalonia.Controls;

namespace Sakuno.ING.Browser;

public interface IBrowserService
{
    Uri? Source { get; }
    bool CanGoBack { get; }
    bool CanGoForward { get; }

    Task NavigateAsync(Uri uri, CancellationToken cancellationToken = default);
    Task RefreshAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
    Task GoBackAsync(CancellationToken cancellationToken = default);
    Task GoForwardAsync(CancellationToken cancellationToken = default);

    event EventHandler<WebViewNavigationStartingEventArgs>? NavigationStarted;
    event EventHandler<WebViewNavigationCompletedEventArgs>? NavigationCompleted;
    event EventHandler<WebViewNewWindowRequestedEventArgs>? NewWindowRequested;
}
