using Sakuno.ING.ViewModels.Homeport.Overall;

namespace Sakuno.ING.Shell.Views.Overall;

public partial class Tabs : ReactiveUserControl<TabsViewModel>
{
    public Tabs()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        ViewModel = DependencyInjection.GetContainer(this).GetRequiredService<TabsViewModel>();

        base.OnLoaded(e);
    }
}
