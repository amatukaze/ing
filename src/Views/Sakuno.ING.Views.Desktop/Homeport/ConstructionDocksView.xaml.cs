using ReactiveUI;
using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Homeport;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    [ExportView("ConstructionDocks")]
    public partial class ConstructionDocksView
    {
        public ConstructionDocksView(ConstructionDocksViewModel constructionDocksViewModel)
        {
            InitializeComponent();

            ViewModel = constructionDocksViewModel;

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.ConstructionDocks, v => v.ConstructionDocks.ItemsSource).DisposeWith(disposable);
            });
        }
    }
}
