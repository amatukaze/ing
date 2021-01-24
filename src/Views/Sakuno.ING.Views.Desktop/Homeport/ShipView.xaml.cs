using ReactiveUI;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    public partial class ShipView
    {
        public ShipView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Info.Name, v => v.ShipName.Text).DisposeWith(disposable);
            });
        }
    }
}
