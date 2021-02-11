using ReactiveUI;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    public partial class SlotView
    {
        public SlotView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Item!.Info.IconId, v => v.Icon.Id).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Item!.AerialProficiency, v => v.Proficiency.Proficiency).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Item!.ImprovementLevel, v => v.Improvement.Level).DisposeWith(disposable);

                this.OneWayBind(ViewModel, vm => vm.Item!.Info.PlaneId, v => v.PlaneCount.PlaneId).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.PlaneCount.Current, v => v.PlaneCount.Current).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.PlaneCount.Max, v => v.PlaneCount.Maximum).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.PlaneCount.IsMaximum, v => v.PlaneCount.IsMaximum).DisposeWith(disposable);
            });
        }
    }
}
