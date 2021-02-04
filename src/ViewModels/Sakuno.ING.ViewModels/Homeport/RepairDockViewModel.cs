using ReactiveUI;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.ViewModels.Homeport
{
    public class RepairDockViewModel : ReactiveObject, IDockViewModel
    {
        public RepairDock Model { get; }

        public RepairDockViewModel(RepairDock repairDock)
        {
            Model = repairDock;
        }
    }
}
