using ReactiveUI;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    public partial class AirForceGroupView
    {
        public AirForceGroupView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.CombatRadius, v => v.CombatRadius.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Squadrons, v => v.Squadrons.ItemsSource).DisposeWith(disposable);
            });
        }
    }
}
