using ReactiveUI;
using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Homeport;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    [ExportView("Overall")]
    public partial class OverallView
    {
        public OverallView(OverallViewModel overallViewModel)
        {
            InitializeComponent();

            ViewModel = overallViewModel;

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.ShipCount, v => v.ShipCount.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.MaxShipCount, v => v.MaxShipCount.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.SlotItemCount, v => v.SlotItemCount.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.MaxSlotItemCount, v => v.MaxSlotItemCount.Text).DisposeWith(disposable);
            });
        }
    }
}
