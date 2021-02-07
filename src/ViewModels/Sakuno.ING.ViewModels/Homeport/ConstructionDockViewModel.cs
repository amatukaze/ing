using ReactiveUI;
using Sakuno.ING.Game.Models;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels.Homeport
{
    public class ConstructionDockViewModel : ReactiveObject, IDockViewModel
    {
        public ConstructionDock Model { get; }

        private ObservableAsPropertyHelper<bool> _isCompleted;
        public bool IsCompleted => _isCompleted.Value;

        public ConstructionDockViewModel(ConstructionDock constructionDock)
        {
            Model = constructionDock;

            _isCompleted = constructionDock.WhenAnyValue(r => r.State).Select(r => r == ConstructionDockState.Completed)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(IsCompleted));
        }
    }
}
