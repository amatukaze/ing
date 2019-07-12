using System;

namespace Sakuno.ING.Game.Models.MasterData
{
    public partial class ShipTypeInfo
    {
        partial void UpdateCore(RawShipTypeInfo raw, DateTimeOffset timeStamp) => availableEquipmentTypes.Query = owner.EquipmentTypes[raw.AvailableEquipmentTypes];
    }
}
