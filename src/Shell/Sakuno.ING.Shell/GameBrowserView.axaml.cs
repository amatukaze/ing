using System.Reactive.Disposables.Fluent;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using Sakuno.ING.Browser;
using Sakuno.ING.ViewModels;

namespace Sakuno.ING.Shell;

public partial class GameBrowserView : ReactiveUserControl<BrowserViewModel>
{
    private IServiceScope? _serviceScope;
    private IBrowserService? _browserService;
    private readonly CompositeDisposable _disposables = [];

    public GameBrowserView()
    {
        InitializeComponent();
    }

    protected override void OnInitialized()
    {
        _serviceScope = DependencyInjection.GetContainer(this).CreateScope();

        DependencyInjection.SetContainer(this, _serviceScope.ServiceProvider);

        ViewModel = _serviceScope.ServiceProvider.GetRequiredService<BrowserViewModel>();

        var browserService = new WebViewBrowserService(GameWebView);
        _browserService = browserService;

        browserService.NavigationStarted += OnNavigationStarted;
        _disposables.Add(Disposable.Create(() => browserService.NavigationStarted -= OnNavigationStarted));

        browserService.NavigationCompleted += OnNavigationCompleted;
        _disposables.Add(Disposable.Create(() => browserService.NavigationCompleted -= OnNavigationCompleted));

        ViewModel.Refresh.Subscribe(_ => browserService.RefreshAsync()).DisposeWith(_disposables);
        ViewModel.Stop.Subscribe(_ => browserService.StopAsync()).DisposeWith(_disposables);
        ViewModel.GoBack.Subscribe(_ => browserService.GoBackAsync()).DisposeWith(_disposables);
        ViewModel.GoForward.Subscribe(_ => browserService.GoForwardAsync()).DisposeWith(_disposables);
    }

    private void OnNavigationStarted(object? sender, WebViewNavigationStartingEventArgs e) =>
        ViewModel?.OnNavigationStarted();

    private void OnNavigationCompleted(object? sender, WebViewNavigationCompletedEventArgs e)
    {
        if (_browserService is { } browserService)
            ViewModel?.OnNavigationCompleted(e.Request, browserService.CanGoBack, browserService.CanGoForward, e.IsSuccess);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        _disposables.Dispose();
        _serviceScope?.Dispose();

        base.OnUnloaded(e);
    }

    private void OnAddressBarKeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key is Avalonia.Input.Key.Enter && ViewModel is not null)
            ViewModel.Navigate.Execute(Unit.Default).Subscribe();
    }
}
