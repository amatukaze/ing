using System.Reactive.Disposables.Fluent;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using Sakuno.ING.ViewModels;

namespace Sakuno.ING.Shell;

public partial class GameBrowserView : ReactiveUserControl<BrowserViewModel>
{
    private IServiceScope? _serviceScope;
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

        GameWebView.NavigationStarted += OnNavigationStarted;
        _disposables.Add(Disposable.Create(() => GameWebView.NavigationStarted -= OnNavigationStarted));

        GameWebView.NavigationCompleted += OnNavigationCompleted;
        _disposables.Add(Disposable.Create(() => GameWebView.NavigationCompleted -= OnNavigationCompleted));

        ViewModel.Refresh.Subscribe(_ => GameWebView.Refresh()).DisposeWith(_disposables);
        ViewModel.Stop.Subscribe(_ => GameWebView.Stop()).DisposeWith(_disposables);
        ViewModel.GoBack.Subscribe(_ => GameWebView.GoBack()).DisposeWith(_disposables);
        ViewModel.GoForward.Subscribe(_ => GameWebView.GoForward()).DisposeWith(_disposables);
    }

    private void OnNavigationStarted(object? sender, WebViewNavigationStartingEventArgs e) =>
        ViewModel?.OnNavigationStarted();

    private void OnNavigationCompleted(object? sender, WebViewNavigationCompletedEventArgs e) =>
        ViewModel?.OnNavigationCompleted(e.Request, GameWebView.CanGoBack, GameWebView.CanGoForward, e.IsSuccess);

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
