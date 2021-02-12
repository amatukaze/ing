using ReactiveUI;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    public partial class FleetView
    {
        public FleetView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Ships, v => v.Ships.ItemsSource).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.TotalLevel, v => v.TotalLevel.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Speed, v => v.Speed.Text).DisposeWith(disposable);

                this.OneWayBind(ViewModel, vm => vm.TotalFirepower, v => v.TotalFirepower.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.TotalAntiAir, v => v.TotalAntiAir.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.TotalAntiSubmarine, v => v.TotalAntiSubmarine.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.TotalLoS, v => v.TotalLoS.Text).DisposeWith(disposable);
            });
        }
    }
}
