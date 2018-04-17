using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    partial class EquipmentInfo
    {
        partial void UpdateCore(IRawEquipmentInfo raw)
        {
            Type = equipmentTypeInfos[raw.TypeId];
            ExtraSlotAcceptingShips = raw.ExtraSlotAcceptingShips.Select(x => shipInfos.TryGetOrDummy(x)).ToArray();
        }
    }
}
