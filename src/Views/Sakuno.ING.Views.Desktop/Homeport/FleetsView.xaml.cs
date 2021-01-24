using ReactiveUI;
using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Homeport;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    [ExportView("Fleets")]
    public partial class FleetsView
    {
        public FleetsView(FleetsViewModel fleetsViewModel)
        {
            InitializeComponent();

            ViewModel = fleetsViewModel;

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Fleets, v => v.Fleets.ItemsSource).DisposeWith(disposable);
            });
        }
    }
}
