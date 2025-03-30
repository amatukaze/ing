using Sakuno.ING.ViewModels.Homeport.Overall;

namespace Sakuno.ING.Shell.Views.Overall;

[RegisterTransient<IViewFor<ShipViewModel>>]
public partial class Ship : ReactiveUserControl<ShipViewModel>
{
    public Ship()
    {
        InitializeComponent();
    }
}
