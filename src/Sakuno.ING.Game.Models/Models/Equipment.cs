namespace Sakuno.ING.Game.Models
{
    partial class Equipment
    {
        partial void UpdateCore(IRawEquipment raw)
        {
            Info = owner.MasterData.EquipmentInfos[raw.EquipmentInfoId];
        }
    }
}
