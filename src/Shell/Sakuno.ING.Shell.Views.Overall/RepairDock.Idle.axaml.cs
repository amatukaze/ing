namespace Sakuno.ING.Shell.Views.Overall;

[RegisterTransient<IViewFor<RepairDockViewModel>>(ServiceKey = nameof(RepairDockState.Idle))]
public partial class RepairDockIdle : ReactiveUserControl<RepairDockViewModel>
{
    public RepairDockIdle()
    {
        InitializeComponent();
    }
}
