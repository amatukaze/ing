namespace Sakuno.ING.Shell.Views.Overall;

[RegisterTransient<IViewFor<RepairDockViewModel>>(ServiceKey = nameof(RepairDockState.Repairing))]
public partial class RepairDockRepairing : ReactiveUserControl<RepairDockViewModel>
{
    public RepairDockRepairing()
    {
        InitializeComponent();
    }
}
