using ReactiveUI;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.ViewModels.Homeport
{
    public class ConstructionDockViewModel : ReactiveObject, IDockViewModel
    {
        public ConstructionDock Model { get; }

        public ConstructionDockViewModel(ConstructionDock ConstructionDock)
        {
            Model = ConstructionDock;
        }
    }
}
