using Sakuno.ING.ViewModels;

namespace Sakuno.ING.Shell;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow(MainWindowViewModel vm)
    {
        ViewModel = vm;

        InitializeComponent();

        this.WhenActivated(disposables =>
        {
        });
    }
}
