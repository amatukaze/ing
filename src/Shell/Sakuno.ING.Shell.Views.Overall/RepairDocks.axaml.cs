namespace Sakuno.ING.Shell.Views.Overall;

public partial class RepairDocks : ReactiveUserControl<RepairDocksViewModel>
{
    public RepairDocks()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        ViewModel = DependencyInjection.GetContainer(this).GetRequiredService<RepairDocksViewModel>();
    }
}
