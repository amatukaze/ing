using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public sealed class ConstructionStart : IMaterialUpdate
    {
        public ConstructionDockId DockId { get; }
        public bool InstantBuild { get; }
        public bool IsLSC { get; }
        public Materials Consumption { get; }

        public ConstructionStart(ConstructionDockId dockId, bool instantBuild, bool isLSC, Materials consumption)
        {
            DockId = dockId;
            InstantBuild = instantBuild;
            IsLSC = isLSC;
            Consumption = consumption;
        }

        public void Apply(ref Materials materials)
        {
            materials.Fuel -= Consumption.Fuel;
            materials.Bullet -= Consumption.Bullet;
            materials.Steel -= Consumption.Steel;
            materials.Bauxite -= Consumption.Bauxite;
            materials.Development -= Consumption.Development;

            if (InstantBuild)
                materials.InstantBuild -= IsLSC ? 10 : 1;
        }
    }
}
