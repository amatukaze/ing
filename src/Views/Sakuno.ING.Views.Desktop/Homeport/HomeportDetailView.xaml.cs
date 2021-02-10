using ReactiveUI;
using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Homeport;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    [ExportView("HomeportDetail")]
    public partial class HomeportDetailView
    {
        public HomeportDetailView(HomeportDetailViewModel homeportDetailViewModel)
        {
            InitializeComponent();

            ViewModel = homeportDetailViewModel;

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Fleets, v => v.Fleets.ItemsSource).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Areas, v => v.Areas.ItemsSource).DisposeWith(disposable);

                this.OneWayBind(ViewModel, vm => vm.SelectedTab, v => v.SelectedTab.Content).DisposeWith(disposable);
            });
        }
    }
}
