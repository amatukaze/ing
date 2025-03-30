namespace Sakuno.ING.Shell.Views.Overall;

public partial class ConstructionDocks : ReactiveUserControl<ConstructionDocksViewModel>
{
    public ConstructionDocks()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        ViewModel = DependencyInjection.GetContainer(this).GetRequiredService<ConstructionDocksViewModel>();
    }
}
