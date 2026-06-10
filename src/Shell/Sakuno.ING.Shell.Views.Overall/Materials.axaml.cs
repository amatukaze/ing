namespace Sakuno.ING.Shell.Views.Overall;

public partial class Materials : ReactiveUserControl<MaterialsViewModel>
{
    public Materials()
    {
        InitializeComponent();
    }

    protected override void OnInitialized()
    {
        ViewModel = DependencyInjection.GetContainer(this).GetRequiredService<MaterialsViewModel>();
    }
}
