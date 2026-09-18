using Avalonia.Controls;

namespace Sakuno.ING.Browser;

public sealed class WebViewBrowserService : IBrowserService
{
    private readonly NativeWebView _webView;

    public Uri? Source => _webView.Source;
    public bool CanGoBack => _webView.CanGoBack;
    public bool CanGoForward => _webView.CanGoForward;

    public event EventHandler<WebViewNavigationStartingEventArgs>? NavigationStarted;
    public event EventHandler<WebViewNavigationCompletedEventArgs>? NavigationCompleted;
    public event EventHandler<WebViewNewWindowRequestedEventArgs>? NewWindowRequested;

    public WebViewBrowserService(NativeWebView webView)
    {
        _webView = webView;

        _webView.NavigationStarted += (sender, args) => NavigationStarted?.Invoke(sender, args);
        _webView.NavigationCompleted += (sender, args) => NavigationCompleted?.Invoke(sender, args);
        _webView.NewWindowRequested += (sender, args) => NewWindowRequested?.Invoke(sender, args);
    }

    public Task NavigateAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        _webView.Navigate(uri);

        return Task.CompletedTask;
    }

    public Task RefreshAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(_webView.Refresh());

    public Task StopAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(_webView.Stop());

    public Task GoBackAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(_webView.GoBack());

    public Task GoForwardAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(_webView.GoForward());
}
