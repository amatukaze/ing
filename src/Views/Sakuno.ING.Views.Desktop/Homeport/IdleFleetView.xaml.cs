using ReactiveUI;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    public partial class IdleFleetView
    {
        public IdleFleetView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Id, v => v.Id.Text).DisposeWith(disposable);

                this.OneWayBind(ViewModel, vm => vm.ShouldSupply, v => v.ShouldSupplyState.Visibility).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.ShouldSupply, v => v.IdleState.Visibility, BooleanToVisibilityHint.Inverse).DisposeWith(disposable);
            });
        }
    }
}
