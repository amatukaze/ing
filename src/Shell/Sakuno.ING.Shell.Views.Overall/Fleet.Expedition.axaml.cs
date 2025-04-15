namespace Sakuno.ING.Shell.Views.Overall;

[RegisterTransient<IViewFor<FleetViewModel>>(ServiceKey = nameof(FleetState.Expedition))]
public partial class FleetExpedition : ReactiveUserControl<FleetViewModel>
{
    public FleetExpedition()
    {
        InitializeComponent();
    }
}
