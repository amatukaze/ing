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
                this.OneWayBind(ViewModel, vm => vm.Model.Id, v => v.Id.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Model.RepairingShip.Info.Name, v => v.Ship.Text).DisposeWith(disposable);
            });
        }
    }
}
