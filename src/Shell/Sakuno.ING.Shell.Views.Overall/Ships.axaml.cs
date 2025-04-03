using Sakuno.ING.ViewModels.Homeport.Overall;

namespace Sakuno.ING.Shell.Views.Overall;

public partial class Ships : ReactiveUserControl<SelectedFleetViewModel>
{
    public Ships()
    {
        InitializeComponent();
    }

    protected override void OnInitialized()
    {
        ViewModel = DependencyInjection.GetContainer(this).GetRequiredService<SelectedFleetViewModel>();
    }
}
