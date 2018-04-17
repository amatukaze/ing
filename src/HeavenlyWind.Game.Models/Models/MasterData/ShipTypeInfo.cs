using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    partial class ShipTypeInfo
    {
        partial void UpdateCore(IRawShipTypeInfo raw)
        {
            availableEquipmentTypes.Query = raw.AvailableEquipmentTypes.Select(equipmentTypeInfos.TryGetOrDummy);
        }
    }
}
