using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public class AerialSide
    {
        public AerialSide(MasterDataRoot masterData, in RawAerialSide raw, Side side)
        {
            FightedPlanes = raw.FightedPlanes;
            ShootedPlanes = raw.ShootedPlanes;
            TouchingPlane = masterData.EquipmentInfos[raw.TouchingPlane];
            PlanesFrom = raw.PlanesFrom.Select(side.FindShip).ToArray();
        }

        public ClampedValue FightedPlanes { get; }
        public ClampedValue ShootedPlanes { get; }
        public EquipmentInfo TouchingPlane { get; }
        public IReadOnlyCollection<BattleParticipant> PlanesFrom { get; }
    }
}
