using ReactiveUI;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    public partial class LockedDockView
    {
        public LockedDockView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Id, v => v.Id.Text).DisposeWith(disposable);
            });
        }
    }
}
