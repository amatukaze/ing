using ReactiveUI;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    public partial class FleetOnExpeditionView
    {
        public FleetOnExpeditionView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Model.Id, v => v.Id.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Model.Expedition.Name, v => v.Expedition.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Model.ExpeditionCompletionTime, v => v.RemainingTime.Time).DisposeWith(disposable);
            });
        }
    }
}
