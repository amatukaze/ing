using ReactiveUI;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    [ViewContract("OnExpedition")]
    public partial class FleetOnExpeditionView
    {
        public FleetOnExpeditionView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Id, v => v.Id.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Expedition.Name, v => v.Expedition.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.CompletionTime, v => v.RemainingTime.Time).DisposeWith(disposable);
            });
        }
    }
}
