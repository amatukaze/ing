using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public class NightEffects
    {
        public NightEffects(MasterDataRoot masterData, IReadOnlyList<BattleParticipant> fleet, in RawNightEffects raw)
        {
            if (raw.FlareIndex is int i)
                FlareShootingShip = fleet[i];
            TouchingPlane = masterData.EquipmentInfos[raw.TouchingPlane];
        }

        public BattleParticipant FlareShootingShip { get; }
        public EquipmentInfo TouchingPlane { get; }
    }
}
