using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public class ShipCreation : IMaterialsUpdate
    {
        public ShipCreation(BuildingDockId buildingDockId, bool instantBuild, bool isLSC, Materials consumption)
        {
            BuildingDockId = buildingDockId;
            InstantBuild = instantBuild;
            IsLSC = isLSC;
            Consumption = consumption;
        }

        public BuildingDockId BuildingDockId { get; }
        public bool InstantBuild { get; }
        public bool IsLSC { get; }
        public Materials Consumption { get; }

        public MaterialsChangeReason Reason => MaterialsChangeReason.ShipCreate;

        public void Apply(ref Materials materials)
        {
            materials.Fuel -= Consumption.Fuel;
            materials.Bullet -= Consumption.Bullet;
            materials.Steel -= Consumption.Steel;
            materials.Bauxite -= Consumption.Bauxite;

            if (InstantBuild)
                materials.InstantBuild -= IsLSC ? 10 : 1;
        }
    }
}
