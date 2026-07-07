using Avalonia.Controls;

namespace Sakuno.ING.Shell.Views.Overall;

public partial class OverallView : UserControl
{
    private IServiceScope? _serviceScope;

    public OverallView()
    {
        InitializeComponent();
    }

    protected override void OnInitialized()
    {
        _serviceScope = DependencyInjection.GetContainer(this).CreateScope();

        DependencyInjection.SetContainer(this, _serviceScope.ServiceProvider);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        _serviceScope?.Dispose();

        base.OnUnloaded(e);
    }
}
