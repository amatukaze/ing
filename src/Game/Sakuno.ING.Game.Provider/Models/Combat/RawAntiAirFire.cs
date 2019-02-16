using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public class RawAntiAirFire
    {
        public RawAntiAirFire(int index, int type, IReadOnlyList<EquipmentInfoId> equipmentUsed)
        {
            Index = index;
            Type = type;
            EquipmentUsed = equipmentUsed;
        }

        public int Index { get; }
        public int Type { get; }
        public IReadOnlyList<EquipmentInfoId> EquipmentUsed { get; }
    }
}
