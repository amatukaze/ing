namespace Sakuno.ING.Shell.Views.Overall;

[RegisterTransient<IViewFor<FleetViewModel>>(ServiceKey = nameof(FleetState.Idle))]
public partial class FleetIdle : ReactiveUserControl<FleetViewModel>
{
    public FleetIdle()
    {
        InitializeComponent();
    }
}
