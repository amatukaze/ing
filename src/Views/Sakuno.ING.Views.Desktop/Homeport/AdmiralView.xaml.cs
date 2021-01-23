using ReactiveUI;
using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Homeport;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    [ExportView("Admiral")]
    public partial class AdmiralView
    {
        public AdmiralView(AdmiralViewModel admiralViewModel)
        {
            InitializeComponent();

            ViewModel = admiralViewModel;

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.AdmiralName.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Level, v => v.Level.Text).DisposeWith(disposable);
            });
        }
    }
}
