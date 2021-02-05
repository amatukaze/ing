using ReactiveUI;
using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Quests;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    [ExportView("ActiveQuests")]
    public partial class ActiveQuestsView
    {
        public ActiveQuestsView(ActiveQuestsViewModel activeQuestsViewModel)
        {
            InitializeComponent();

            ViewModel = activeQuestsViewModel;

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Quests, v => v.Quests.ItemsSource).DisposeWith(disposable);
            });
        }
    }
}
