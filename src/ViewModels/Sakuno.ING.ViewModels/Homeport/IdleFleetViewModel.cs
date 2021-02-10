using ReactiveUI;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.ViewModels.Homeport
{
    public sealed class IdleFleetViewModel : ReactiveObject, IFleetViewModel
    {
        public PlayerFleet Model { get; }

        public IdleFleetViewModel(PlayerFleet fleet)
        {
            Model = fleet;
        }
    }
}
