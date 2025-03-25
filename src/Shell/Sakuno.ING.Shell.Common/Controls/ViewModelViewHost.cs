using System.Reactive.Disposables;
using Splat;

namespace Sakuno.ING.Shell.Controls;

public sealed class ViewModelViewHost : ContentControl, IViewFor, IEnableLogger
{
    public static readonly AvaloniaProperty<object?> ViewModelProperty =
        AvaloniaProperty.Register<ViewModelViewHost, object?>(nameof(ViewModel));

    public static readonly StyledProperty<string?> ViewContractProperty =
        AvaloniaProperty.Register<ViewModelViewHost, string?>(nameof(ViewContract));

    public static readonly StyledProperty<object?> DefaultContentProperty =
        AvaloniaProperty.Register<ViewModelViewHost, object?>(nameof(DefaultContent));

    public object? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public string? ViewContract
    {
        get => GetValue(ViewContractProperty);
        set => SetValue(ViewContractProperty, value);
    }

    public object? DefaultContent
    {
        get => GetValue(DefaultContentProperty);
        set => SetValue(DefaultContentProperty, value);
    }

    public IViewLocator? ViewLocator { get; set; }

    protected override Type StyleKeyOverride => typeof(ContentControl);

    public ViewModelViewHost()
    {
        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(x => x.ViewModel, x => x.ViewContract)
                .Subscribe(tuple => ResolveViewForViewModel(tuple.Item1, tuple.Item2))
                .DisposeWith(disposables);
        });
    }

    private void ResolveViewForViewModel(object? viewModel, string? contract)
    {
        if (viewModel is null)
        {
            this.Log().Info("ViewModel is null. Falling back to default content.");

            Content = DefaultContent;
            return;
        }

        var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
        var viewInstance = viewLocator.ResolveView(viewModel, contract);
        if (viewInstance is null)
        {
            this.Log().Warn(
                contract is null
                    ? $"Couldn't find view for '{viewModel}'. Is it registered? Falling back to default content."
                    : $"Couldn't find view with contract '{contract}' for '{viewModel}'. Is it registered? Falling back to default content.");

            Content = DefaultContent;
            return;
        }

        this.Log().Info(contract is null
            ? $"Ready to show {viewInstance} with autowired {viewModel}."
            : $"Ready to show {viewInstance} with autowired {viewModel} and contract '{contract}'.");

        viewInstance.ViewModel = viewModel;

        if (viewInstance is StyledElement styled)
            styled.DataContext = viewModel;

        Content = viewInstance;
    }
}
