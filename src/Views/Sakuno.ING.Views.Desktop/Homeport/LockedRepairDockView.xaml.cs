using ReactiveUI;
using Sakuno.ING.Game.Models;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    [ViewContract(nameof(RepairDockState.Locked))]
    public partial class LockedRepairDockView
    {
        public LockedRepairDockView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Id, v => v.Id.Text).DisposeWith(disposable);
            });
        }
    }
}
