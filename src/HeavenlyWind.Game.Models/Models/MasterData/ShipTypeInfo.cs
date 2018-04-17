using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    partial class ShipTypeInfo
    {
        partial void UpdateCore(IRawShipTypeInfo raw)
        {
            AvailableEquipmentTypes = raw.AvailableEquipmentTypes.Select(x => equipmentTypeInfos.TryGetOrDummy(x)).ToArray();
        }
    }
}
