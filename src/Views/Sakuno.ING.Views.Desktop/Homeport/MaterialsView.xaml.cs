using ReactiveUI;
using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Homeport;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    [ExportView("Materials")]
    public partial class MaterialsView
    {
        public MaterialsView(MaterialsViewModel materialsViewModel)
        {
            InitializeComponent();

            ViewModel = materialsViewModel;

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Fuel, v => v.Fuel.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Bullet, v => v.Bullet.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Steel, v => v.Steel.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Bauxite, v => v.Bauxite.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.InstantRepair, v => v.InstantRepair.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.InstantBuild, v => v.InstantBuild.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Development, v => v.Development.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Improvement, v => v.Improvement.Text).DisposeWith(disposable);
            });
        }
    }
}
