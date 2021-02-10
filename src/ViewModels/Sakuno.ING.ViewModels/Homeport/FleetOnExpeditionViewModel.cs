using ReactiveUI;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.ViewModels.Homeport
{
    public sealed class FleetOnExpeditionViewModel : ReactiveObject, IFleetViewModel
    {
        public PlayerFleet Model { get; }

        public FleetOnExpeditionViewModel(PlayerFleet fleet)
        {
            Model = fleet;
        }
    }
}
