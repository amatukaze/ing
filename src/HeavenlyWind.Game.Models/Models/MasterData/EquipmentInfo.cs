using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    partial class EquipmentInfo
    {
        partial void UpdateCore(IRawEquipmentInfo raw)
        {
            Type = equipmentTypeInfos[raw.TypeId];
            extraSlotAcceptingShips.Query = raw.ExtraSlotAcceptingShips.Select(shipInfos.TryGetOrDummy);
        }
    }
}
