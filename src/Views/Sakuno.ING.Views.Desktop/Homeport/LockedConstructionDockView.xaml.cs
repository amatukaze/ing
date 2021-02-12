using ReactiveUI;
using Sakuno.ING.Game.Models;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    [ViewContract(nameof(ConstructionDockState.Locked))]
    public partial class LockedConstructionDockView
    {
        public LockedConstructionDockView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Id, v => v.Id.Text).DisposeWith(disposable);
            });
        }
    }
}
