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
                this.OneWayBind(ViewModel, vm => vm.Model.Id, v => v.Id.Text).DisposeWith(disposable);
            });
        }
    }
}
