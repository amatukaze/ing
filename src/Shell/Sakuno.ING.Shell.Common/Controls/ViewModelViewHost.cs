using System.Reactive.Linq;
using Avalonia.Interactivity;
using Splat;

namespace Sakuno.ING.Shell.Controls;

public sealed class ViewModelViewHost : ContentControl, IViewFor, IEnableLogger
{
    public static readonly AvaloniaProperty<object?> ViewModelProperty =
        AvaloniaProperty.Register<ViewModelViewHost, object?>(nameof(ViewModel));

    public static readonly StyledProperty<object?> DefaultContentProperty =
        AvaloniaProperty.Register<ViewModelViewHost, object?>(nameof(DefaultContent));

    public object? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public object? DefaultContent
    {
        get => GetValue(DefaultContentProperty);
        set => SetValue(DefaultContentProperty, value);
    }

    public IViewLocator? ViewLocator { get; set; }

    private IDisposable? _subscription;

    protected override void OnInitialized()
    {
        var viewModelSource = this.GetObservable(ViewModelProperty);
        var contractSource = viewModelSource.Select(vm =>
        {
            if (vm is not IViewContractObservable viewContractObservable)
                return Observable.Return<string?>(null);

            return viewContractObservable.ViewContractObservable.ObserveOn(RxSchedulers.MainThreadScheduler);
        }).Switch();

        _subscription = viewModelSource
            .CombineLatest(contractSource, (vm, contract) => (ViewModel: vm, Contract: contract))
            .DistinctUntilChanged()
            .Subscribe(x => ResolveViewForViewModel(x.ViewModel, x.Contract));
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        _subscription?.Dispose();

        base.OnUnloaded(e);
    }

    private void ResolveViewForViewModel(object? viewModel, string? contract)
    {
        if (viewModel is null)
        {
            Content = DefaultContent;
            return;
        }

        var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
        var viewInstance = viewLocator.ResolveView(viewModel, contract);
        if (viewInstance is null)
        {
            this.Log().Warn(contract is null
                ? $"Couldn't find view for '{viewModel}'."
                : $"Couldn't find view with contract '{contract}' for '{viewModel}'.");

            Content = DefaultContent;
            return;
        }

        viewInstance.ViewModel = viewModel;

        Content = viewInstance;
    }
}
