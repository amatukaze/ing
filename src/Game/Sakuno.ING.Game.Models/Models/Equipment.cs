using System;

namespace Sakuno.ING.Game.Models
{
    public partial class Equipment
    {
        partial void UpdateCore(IRawEquipment raw, DateTimeOffset timeStamp)
        {
            Info = owner.MasterData.EquipmentInfos[raw.EquipmentInfoId];
        }
    }
}
