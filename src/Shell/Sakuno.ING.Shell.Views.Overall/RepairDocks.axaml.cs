namespace Sakuno.ING.Shell.Views.Overall;

public partial class RepairDocks : ReactiveUserControl<RepairDocksViewModel>
{
    public RepairDocks()
    {
        InitializeComponent();
    }

    protected override void OnInitialized()
    {
        ViewModel = DependencyInjection.GetContainer(this).GetRequiredService<RepairDocksViewModel>();
    }
}
