using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events
{
    public class MasterDataUpdate
    {
        public MasterDataUpdate
        (
            IReadOnlyCollection<IRawShipInfo> shipInfos,
            IReadOnlyCollection<IRawShipTypeInfo> shipTypes,
            IReadOnlyCollection<IRawEquipmentTypeInfo> equipmentTypes,
            IReadOnlyCollection<IRawEquipmentInfo> equipmentInfos,
            IReadOnlyCollection<IRawFurnitureInfo> furnitureInfos,
            IReadOnlyCollection<IRawUseItem> useItems,
            IReadOnlyCollection<IRawMapArea> mapAreas,
            IReadOnlyCollection<IRawMapInfo> maps,
            IReadOnlyCollection<IRawExpeditionInfo> expeditions,
            IReadOnlyCollection<IRawBgmInfo> bgms,
            IReadOnlyCollection<IRawShipEquipLimitation> shipEquipLimitations
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
            ShipEquipLimitations = shipEquipLimitations;
        }

        public IReadOnlyCollection<IRawShipInfo> ShipInfos { get; }
        public IReadOnlyCollection<IRawShipTypeInfo> ShipTypes { get; }
        public IReadOnlyCollection<IRawEquipmentTypeInfo> EquipmentTypes { get; }
        public IReadOnlyCollection<IRawEquipmentInfo> EquipmentInfos { get; }
        public IReadOnlyCollection<IRawFurnitureInfo> FurnitureInfos { get; }
        public IReadOnlyCollection<IRawUseItem> UseItems { get; }
        public IReadOnlyCollection<IRawMapArea> MapAreas { get; }
        public IReadOnlyCollection<IRawMapInfo> Maps { get; }
        public IReadOnlyCollection<IRawExpeditionInfo> Expeditions { get; }
        public IReadOnlyCollection<IRawBgmInfo> Bgms { get; }
        public IReadOnlyCollection<IRawShipEquipLimitation> ShipEquipLimitations { get; }
    }
}
