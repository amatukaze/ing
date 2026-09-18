namespace Sakuno.ING.ViewModels;

[RegisterScoped(Registration = RegistrationStrategy.Self)]
public class BrowserViewModel : ViewModelObject
{
    public const string GameUrl = "https://games.dmm.com/detail/kancolle";

    private Uri? _source;
    public Uri? Source
    {
        get => _source;
        set => SetField(ref _source, value);
    }

    private string? _address;
    public string? Address
    {
        get => _address;
        set => SetField(ref _address, value);
    }

    private bool _isNavigating;
    public bool IsNavigating
    {
        get => _isNavigating;
        private set => SetField(ref _isNavigating, value);
    }

    private bool _canGoBack;
    public bool CanGoBack
    {
        get => _canGoBack;
        private set => SetField(ref _canGoBack, value);
    }

    private bool _canGoForward;
    public bool CanGoForward
    {
        get => _canGoForward;
        private set => SetField(ref _canGoForward, value);
    }

    public ReactiveCommand<Unit, Unit> Refresh { get; }
    public ReactiveCommand<Unit, Unit> Stop { get; }
    public ReactiveCommand<Unit, Unit> GoHome { get; }
    public ReactiveCommand<Unit, Unit> GoBack { get; }
    public ReactiveCommand<Unit, Unit> GoForward { get; }
    public ReactiveCommand<Unit, Unit> Navigate { get; }

    public BrowserViewModel()
    {
        Source = new Uri(GameUrl);
        Address = GameUrl;

        Refresh = ReactiveCommand.Create(() => Unit.Default);
        Stop = ReactiveCommand.Create(() => Unit.Default);
        Navigate = ReactiveCommand.Create(() =>
        {
            if (Uri.TryCreate(Address, UriKind.Absolute, out var uri))
                Source = uri;

            return Unit.Default;
        });
        GoHome = ReactiveCommand.Create(() =>
        {
            Source = new Uri(GameUrl);

            return Unit.Default;
        });
        GoBack = ReactiveCommand.Create(() => Unit.Default, this.WhenAnyValue(r => r.CanGoBack));
        GoForward = ReactiveCommand.Create(() => Unit.Default, this.WhenAnyValue(r => r.CanGoForward));
    }

    public void OnNavigationStarted() => IsNavigating = true;

    public void OnNavigationCompleted(Uri? uri, bool canGoBack, bool canGoForward, bool isSuccess)
    {
        if (isSuccess)
            Address = uri?.ToString();

        CanGoBack = canGoBack;
        CanGoForward = canGoForward;
        IsNavigating = false;
    }
}
