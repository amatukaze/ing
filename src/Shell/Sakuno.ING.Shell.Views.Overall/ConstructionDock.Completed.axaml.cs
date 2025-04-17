namespace Sakuno.ING.Shell.Views.Overall;

[RegisterTransient<IViewFor<ConstructionDockViewModel>>(ServiceKey = nameof(ConstructionDockState.Completed))]
public partial class ConstructionDockCompleted : ReactiveUserControl<ConstructionDockViewModel>
{
    public ConstructionDockCompleted()
    {
        InitializeComponent();
    }
}
