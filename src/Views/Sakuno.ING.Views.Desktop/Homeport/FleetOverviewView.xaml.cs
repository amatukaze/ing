using ReactiveUI;
using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Homeport;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    [ExportView("FleetOverview")]
    public partial class FleetOverviewView
    {
        public FleetOverviewView(FleetOverviewViewModel fleetsViewModel)
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
