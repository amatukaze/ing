using Sakuno.ING.ViewModels.Homeport.Overall;

namespace Sakuno.ING.Shell.Views.Overall;

[RegisterTransient<IViewFor<FleetTabViewModel>>]
public partial class FleetTab : ReactiveUserControl<FleetTabViewModel>
{
    public FleetTab()
    {
        InitializeComponent();
    }
}
