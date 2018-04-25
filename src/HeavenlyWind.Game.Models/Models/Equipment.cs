namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    partial class Equipment
    {
        partial void UpdateCore(IRawEquipment raw)
        {
            Info = equipmentInfoTable[raw.EquipmentInfoId];
        }
    }
}
