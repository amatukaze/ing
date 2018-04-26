using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events
{
    public class MasterDataUpdate
    {
        public IReadOnlyCollection<IRawShipInfo> ShipInfos { get; internal set; }
        public IReadOnlyCollection<IRawShipTypeInfo> ShipTypes { get; internal set; }
        public IReadOnlyCollection<IRawEquipmentTypeInfo> EquipmentTypes { get; internal set; }
        public IReadOnlyCollection<IRawEquipmentInfo> EquipmentInfos { get; internal set; }
        public IReadOnlyCollection<IRawFurnitureInfo> FurnitureInfos { get; internal set; }
        public IReadOnlyCollection<IRawUseItem> UseItems { get; internal set; }
        public IReadOnlyCollection<IRawMapArea> MapAreas { get; internal set; }
        public IReadOnlyCollection<IRawMapInfo> Maps { get; internal set; }
        public IReadOnlyCollection<IRawExpeditionInfo> Expeditions { get; internal set; }
        public IReadOnlyCollection<IRawBgmInfo> Bgms { get; internal set; }
    }
}
