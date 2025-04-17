namespace Sakuno.ING.Shell.Views.Overall;

[RegisterTransient<IViewFor<ConstructionDockViewModel>>(ServiceKey = nameof(ConstructionDockState.Idle))]
public partial class ConstructionDockIdle : ReactiveUserControl<ConstructionDockViewModel>
{
    public ConstructionDockIdle()
    {
        InitializeComponent();
    }
}
