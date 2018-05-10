using System.Linq;

namespace Sakuno.ING.Game.Models.MasterData
{
    partial class EquipmentInfo
    {
        partial void UpdateCore(IRawEquipmentInfo raw)
        {
            Type = owner.EquipmentTypes[raw.TypeId];
            extraSlotAcceptingShips.Query = raw.ExtraSlotAcceptingShips.Select(owner.ShipInfos.TryGetOrDummy);
        }
    }
}
