using System.Windows.Controls;
using System.Windows;
using ReactiveUI;
using Splat;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace Sakuno.ING.Shell.Controls;

public sealed class ViewModelViewHost : ContentControl, IViewFor, IEnableLogger
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(object), typeof(ViewModelViewHost), new PropertyMetadata(null));

    public object? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public static readonly DependencyProperty DefaultContentProperty =
        DependencyProperty.Register(nameof(DefaultContent), typeof(object), typeof(ViewModelViewHost), new PropertyMetadata(null));

    public object? DefaultContent
    {
        get => GetValue(DefaultContentProperty);
        set => SetValue(DefaultContentProperty, value);
    }

    public ViewModelViewHost()
    {
        var viewModelChanged = this.WhenAnyValue(r => r.ViewModel).StartWith(ViewModel);
        var contractChanged = this.WhenAnyValue(r => r.ViewModel).Select(vm =>
        {
            if (vm is not IViewContractObservable viewContractObservable)
                return Observable.Return<string?>(null);

            return viewContractObservable.ViewContractObservable.ObserveOn(RxApp.MainThreadScheduler);
        }).Switch();
        var vmAndContract = viewModelChanged
            .CombineLatest(contractChanged, (vm, contract) => (ViewModel: vm, Contract: contract));

        this.WhenActivated(disposable =>
        {
            vmAndContract.DistinctUntilChanged()
                .Subscribe(x => ResolveViewForViewModel(x.ViewModel, x.Contract))
                .DisposeWith(disposable);
        });
    }

    private void ResolveViewForViewModel(object? viewModel, string? contract)
    {
        if (viewModel is null)
        {
            Content = DefaultContent;
            return;
        }

        var viewInstance = ViewLocator.Current.ResolveView(viewModel, contract);

        if (viewInstance is null)
        {
            Content = DefaultContent;
            this.Log().Warn($"The {nameof(ViewModelViewHost)} could not find a valid view for the view model of type {viewModel.GetType()} and value {viewModel}.");
            return;
        }

        viewInstance.ViewModel = viewModel;

        Content = viewInstance;
    }
}
