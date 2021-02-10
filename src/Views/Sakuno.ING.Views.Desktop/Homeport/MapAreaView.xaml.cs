using ReactiveUI;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    public partial class MapAreaView
    {
        public MapAreaView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Groups, v => v.Groups.ItemsSource).DisposeWith(disposable);
            });
        }
    }
}
