namespace Sakuno.ING.Shell.Views.Overall;

public partial class SlotItemCount : ReactiveUserControl<SlotItemCountViewModel>
{
    public SlotItemCount()
    {
        InitializeComponent();
    }

    protected override void OnInitialized()
    {
        ViewModel = DependencyInjection.GetContainer(this).GetRequiredService<SlotItemCountViewModel>();
    }
}
