using ReactiveUI;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    public partial class ShipView
    {
        public ShipView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Info.Name, v => v.ShipName.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Level, v => v.Level.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.NextExperience, v => v.NextExperience.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.CurrentHP, v => v.CurrentHP.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.MaxHP, v => v.MaxHP.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Slots, v => v.Slots.ItemsSource).DisposeWith(disposable);
            });
        }
    }
}
