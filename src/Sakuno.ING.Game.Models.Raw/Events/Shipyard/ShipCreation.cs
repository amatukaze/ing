using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public class ShipCreation
    {
        public ShipCreation(int buildingDockId, bool instantBuild, bool isLSC, Materials consumption)
        {
            BuildingDockId = buildingDockId;
            InstantBuild = instantBuild;
            IsLSC = isLSC;
            Consumption = consumption;
        }

        public int BuildingDockId { get; }
        public bool InstantBuild { get; }
        public bool IsLSC { get; }
        public Materials Consumption { get; }
    }
}
