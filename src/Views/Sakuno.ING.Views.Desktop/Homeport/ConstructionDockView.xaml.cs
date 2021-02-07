using ReactiveUI;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    public partial class ConstructionDockView
    {
        public ConstructionDockView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Model.Id, v => v.Id.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Model.BuiltShip.Name, v => v.Ship.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Model.CompletionTime, v => v.RemainingTime.Time).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Model.Consumption.Fuel, v => v.FuelConsumption.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Model.Consumption.Bullet, v => v.BulletConsumption.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Model.Consumption.Steel, v => v.SteelConsumption.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Model.Consumption.Bauxite, v => v.BauxiteConsumption.Text).DisposeWith(disposable);

                this.OneWayBind(ViewModel, vm => vm.IsCompleted, v => v.RemainingTime.Visibility, BooleanToVisibilityHint.Inverse).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.IsCompleted, v => v.CompletionText.Visibility).DisposeWith(disposable);
            });
        }
    }
}
