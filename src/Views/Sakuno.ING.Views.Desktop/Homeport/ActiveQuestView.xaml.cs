using ReactiveUI;
using System.Reactive.Disposables;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    public partial class ActiveQuestView
    {
        public ActiveQuestView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.QuestName.Text).DisposeWith(disposable);
            });
        }
    }
}
