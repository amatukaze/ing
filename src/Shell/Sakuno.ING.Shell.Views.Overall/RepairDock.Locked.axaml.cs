namespace Sakuno.ING.Shell.Views.Overall;

[RegisterTransient<IViewFor<RepairDockViewModel>>(ServiceKey = nameof(RepairDockState.Locked))]
public partial class RepairDockLocked : ReactiveUserControl<RepairDockViewModel>
{
    public RepairDockLocked()
    {
        InitializeComponent();
    }
}
