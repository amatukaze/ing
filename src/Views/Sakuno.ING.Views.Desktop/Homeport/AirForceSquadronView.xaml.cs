using ReactiveUI;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    public partial class AirForceSquadronView
    {
        public AirForceSquadronView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Model.SlotItem.Info.IconId, v => v.Icon.Id).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Model.SlotItem.Info.Name, v => v.Plane.Text).DisposeWith(disposable);
            });
        }
    }
}
