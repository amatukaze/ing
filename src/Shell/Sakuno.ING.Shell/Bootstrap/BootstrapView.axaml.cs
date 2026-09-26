using Sakuno.ING.ViewModels.Bootstrap;

namespace Sakuno.ING.Shell.Bootstrap;

[RegisterTransient<IViewFor<BootstrapViewModel>>]
public partial class BootstrapView : ReactiveUserControl<BootstrapViewModel>
{
    public BootstrapView()
    {
        InitializeComponent();
    }
}
