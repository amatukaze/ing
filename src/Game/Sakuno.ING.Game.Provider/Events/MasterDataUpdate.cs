using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events
{
    public sealed class MasterDataUpdate
    {
        public MasterDataUpdate
        (
            IReadOnlyCollection<RawShipInfo> shipInfos,
            IReadOnlyCollection<RawShipTypeInfo> shipTypes,
            IReadOnlyCollection<RawEquipmentTypeInfo> equipmentTypes,
            IReadOnlyCollection<RawEquipmentInfo> equipmentInfos,
            IReadOnlyCollection<RawFurnitureInfo> furnitureInfos,
            IReadOnlyCollection<RawUseItem> useItems,
            IReadOnlyCollection<RawMapArea> mapAreas,
            IReadOnlyCollection<RawMapInfo> maps,
            IReadOnlyCollection<RawExpeditionInfo> expeditions,
            IReadOnlyCollection<RawBgmInfo> bgms
        )
        {
            ShipInfos = shipInfos;
            ShipTypes = shipTypes;
            EquipmentTypes = equipmentTypes;
            EquipmentInfos = equipmentInfos;
            FurnitureInfos = furnitureInfos;
            UseItems = useItems;
            MapAreas = mapAreas;
            Maps = maps;
            Expeditions = expeditions;
            Bgms = bgms;
        }

        public IReadOnlyCollection<RawShipInfo> ShipInfos { get; }
        public IReadOnlyCollection<RawShipTypeInfo> ShipTypes { get; }
        public IReadOnlyCollection<RawEquipmentTypeInfo> EquipmentTypes { get; }
        public IReadOnlyCollection<RawEquipmentInfo> EquipmentInfos { get; }
        public IReadOnlyCollection<RawFurnitureInfo> FurnitureInfos { get; }
        public IReadOnlyCollection<RawUseItem> UseItems { get; }
        public IReadOnlyCollection<RawMapArea> MapAreas { get; }
        public IReadOnlyCollection<RawMapInfo> Maps { get; }
        public IReadOnlyCollection<RawExpeditionInfo> Expeditions { get; }
        public IReadOnlyCollection<RawBgmInfo> Bgms { get; }
    }
}
