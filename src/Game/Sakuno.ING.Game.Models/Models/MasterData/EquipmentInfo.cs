using System;
using System.Linq;

namespace Sakuno.ING.Game.Models.MasterData
{
    public partial class EquipmentInfo
    {
        partial void UpdateCore(RawEquipmentInfo raw, DateTimeOffset timeStamp)
        {
            Type = owner.EquipmentTypes[raw.TypeId];
            extraSlotAcceptingShips.Query = raw.ExtraSlotAcceptingShips.Select(x => owner.ShipInfos[x]);
        }
    }
}
