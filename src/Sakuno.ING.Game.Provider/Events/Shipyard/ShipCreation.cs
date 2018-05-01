using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public class ShipCreation
    {
        public int BuildingDockId { get; internal set; }
        public bool InstantBuild { get; internal set; }
        public bool IsLSC { get; internal set; }
        public Materials Consumption { get; internal set; }
    }
}
