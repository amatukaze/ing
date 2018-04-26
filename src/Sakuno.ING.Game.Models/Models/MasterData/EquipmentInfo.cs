using System.Linq;

namespace Sakuno.ING.Game.Models.MasterData
{
    partial class EquipmentInfo
    {
        partial void UpdateCore(IRawEquipmentInfo raw)
        {
            Type = equipmentTypeInfoTable[raw.TypeId];
            extraSlotAcceptingShips.Query = raw.ExtraSlotAcceptingShips.Select(shipInfoTable.TryGetOrDummy);
        }
    }
}
