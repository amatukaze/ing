namespace Sakuno.ING.Shell.Views.Overall;

public partial class ShipCount : ReactiveUserControl<ShipCountViewModel>
{
    public ShipCount()
    {
        InitializeComponent();
    }

    protected override void OnInitialized()
    {
        ViewModel = DependencyInjection.GetContainer(this).GetRequiredService<ShipCountViewModel>();
    }
}
