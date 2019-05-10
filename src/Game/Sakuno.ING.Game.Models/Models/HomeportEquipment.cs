using System;

namespace Sakuno.ING.Game.Models
{
    public partial class HomeportEquipment
    {
        partial void UpdateCore(RawEquipment raw, DateTimeOffset timeStamp)
        {
            Info = owner.MasterData.EquipmentInfos[raw.EquipmentInfoId];
            Slot?.CascadeUpdate();
        }
    }
}
