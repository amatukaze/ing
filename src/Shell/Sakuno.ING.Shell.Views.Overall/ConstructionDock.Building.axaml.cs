namespace Sakuno.ING.Shell.Views.Overall;

[RegisterTransient<IViewFor<ConstructionDockViewModel>>(ServiceKey = nameof(ConstructionDockState.Building))]
public partial class ConstructionDockBuilding : ReactiveUserControl<ConstructionDockViewModel>
{
    public ConstructionDockBuilding()
    {
        InitializeComponent();
    }
}
