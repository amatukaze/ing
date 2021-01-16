using ReactiveUI;
using Sakuno.ING.Composition;
using System.Reactive.Disposables;

namespace Sakuno.ING.Shell.Desktop
{
    [Export]
    internal partial class MainWindow
    {
        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();

            ViewModel = mainViewModel;

            this.WhenActivated(disposable =>
            {
            });
        }
    }
}
