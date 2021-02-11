using ReactiveUI;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.ViewModels.Homeport
{
    public sealed class IdleFleetViewModel : ReactiveObject, IFleetViewModel
    {
        public FleetId Id { get; }

        public IdleFleetViewModel(PlayerFleet fleet)
        {
            Id = fleet.Id;
        }
    }
}
