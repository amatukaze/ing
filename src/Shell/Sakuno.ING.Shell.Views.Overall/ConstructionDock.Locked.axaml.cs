namespace Sakuno.ING.Shell.Views.Overall;

[RegisterTransient<IViewFor<ConstructionDockViewModel>>(ServiceKey = nameof(ConstructionDockState.Locked))]
public partial class ConstructionDockLocked : ReactiveUserControl<ConstructionDockViewModel>
{
    public ConstructionDockLocked()
    {
        InitializeComponent();
    }
}
