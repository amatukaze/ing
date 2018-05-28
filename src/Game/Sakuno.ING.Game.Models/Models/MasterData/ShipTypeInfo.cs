using System.Linq;

namespace Sakuno.ING.Game.Models.MasterData
{
    partial class ShipTypeInfo
    {
        partial void UpdateCore(IRawShipTypeInfo raw)
        {
            availableEquipmentTypes.Query = raw.AvailableEquipmentTypes.Select(x => owner.EquipmentTypes[x]);
        }
    }
}
