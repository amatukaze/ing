using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public class AntiAirFire
    {
        public AntiAirFire(MasterDataRoot masterData, Side ally, RawAntiAirFire raw)
        {
            Ship = ally.FindShip(raw.Index);
            Type = raw.Type;
            EquipmentUsed = raw.EquipmentUsed.Select(x => masterData.EquipmentInfos[x]).ToArray();
        }

        public BattleParticipant Ship { get; }
        public int Type { get; }
        public IReadOnlyList<EquipmentInfo> EquipmentUsed { get; }
    }
}
