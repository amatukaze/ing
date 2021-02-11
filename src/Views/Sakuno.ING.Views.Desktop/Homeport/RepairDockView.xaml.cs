using ReactiveUI;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    public partial class RepairDockView
    {
        public RepairDockView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Id, v => v.Id.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.RepairingShip.Info.Name, v => v.Ship.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.CompletionTime, v => v.RemainingTime.Time).DisposeWith(disposable);
            });
        }
    }
}
