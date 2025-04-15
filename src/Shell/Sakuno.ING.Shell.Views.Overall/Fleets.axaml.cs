namespace Sakuno.ING.Shell.Views.Overall;

public partial class Fleets : ReactiveUserControl<FleetsViewModel>
{
    public Fleets()
    {
        InitializeComponent();
    }

    protected override void OnInitialized()
    {
        ViewModel = DependencyInjection.GetContainer(this).GetRequiredService<FleetsViewModel>();
    }
}
