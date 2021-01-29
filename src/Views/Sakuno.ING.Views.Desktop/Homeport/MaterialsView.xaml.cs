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
                this.OneWayBind(ViewModel, vm => vm.Fuel, v => v.Fuel.Amount).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Bullet, v => v.Bullet.Amount).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Steel, v => v.Steel.Amount).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Bauxite, v => v.Bauxite.Amount).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.InstantRepair, v => v.InstantRepair.Amount).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.InstantBuild, v => v.InstantBuild.Amount).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Development, v => v.Development.Amount).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Improvement, v => v.Improvement.Amount).DisposeWith(disposable);
            });
        }
    }
}
