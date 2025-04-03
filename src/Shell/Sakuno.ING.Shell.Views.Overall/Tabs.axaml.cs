using Sakuno.ING.ViewModels.Homeport.Overall;

namespace Sakuno.ING.Shell.Views.Overall;

public partial class Tabs : ReactiveUserControl<TabsViewModel>
{
    public Tabs()
    {
        InitializeComponent();
    }

    protected override void OnInitialized()
    {
        ViewModel = DependencyInjection.GetContainer(this).GetRequiredService<TabsViewModel>();
    }
}
