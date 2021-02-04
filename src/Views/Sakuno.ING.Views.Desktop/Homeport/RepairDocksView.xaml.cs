using ReactiveUI;
using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Homeport;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    [ExportView("RepairDocks")]
    public partial class RepairDocksView
    {
        public RepairDocksView(RepairDocksViewModel repairDocksViewModel)
        {
            InitializeComponent();

            ViewModel = repairDocksViewModel;

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.RepairDocks, v => v.RepairDocks.ItemsSource).DisposeWith(disposable);
            });
        }
    }
}
