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
            });
        }
    }
}
