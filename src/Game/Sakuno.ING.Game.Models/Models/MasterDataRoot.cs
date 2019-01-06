using System;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Localization;

namespace Sakuno.ING.Game.Models
{
    public class MasterDataRoot
    {
        public ILocalizationService Localization { get; }

        internal MasterDataRoot(GameProvider listener, ILocalizationService localization)
        {
            Localization = localization;

            _shipInfos = new IdTable<ShipInfoId, ShipInfo, RawShipInfo, MasterDataRoot>(this);
            _shipTypes = new IdTable<ShipTypeId, ShipTypeInfo, RawShipTypeInfo, MasterDataRoot>(this);
            _equipmentTypes = new IdTable<EquipmentTypeId, EquipmentTypeInfo, RawEquipmentTypeInfo, MasterDataRoot>(this);
            _equipmentInfos = new IdTable<EquipmentInfoId, EquipmentInfo, RawEquipmentInfo, MasterDataRoot>(this);
            _useItems = new IdTable<UseItemId, UseItemInfo, RawUseItem, MasterDataRoot>(this);
            _mapAreas = new IdTable<MapAreaId, MapAreaInfo, RawMapArea, MasterDataRoot>(this);
            _mapInfos = new IdTable<MapId, MapInfo, RawMapInfo, MasterDataRoot>(this);
            _expeditions = new IdTable<ExpeditionId, ExpeditionInfo, RawExpeditionInfo, MasterDataRoot>(this);

            listener.MasterDataUpdated += OnMasterDataUpdated;
        }

        private void OnMasterDataUpdated(DateTimeOffset timeStamp, MasterDataUpdate message)
        {
            _shipTypes.BatchUpdate(message.ShipTypes, timeStamp);
            _shipInfos.BatchUpdate(message.ShipInfos, timeStamp);
            _equipmentTypes.BatchUpdate(message.EquipmentTypes, timeStamp);
            _equipmentInfos.BatchUpdate(message.EquipmentInfos, timeStamp);
            _useItems.BatchUpdate(message.UseItems, timeStamp);
            _mapAreas.BatchUpdate(message.MapAreas, timeStamp);
            _mapInfos.BatchUpdate(message.Maps, timeStamp);
            _expeditions.BatchUpdate(message.Expeditions, timeStamp);
        }

        private readonly IdTable<ShipInfoId, ShipInfo, RawShipInfo, MasterDataRoot> _shipInfos;
        public ITable<ShipInfoId, ShipInfo> ShipInfos => _shipInfos;

        private readonly IdTable<ShipTypeId, ShipTypeInfo, RawShipTypeInfo, MasterDataRoot> _shipTypes;
        public ITable<ShipTypeId, ShipTypeInfo> ShipTypes => _shipTypes;

        private readonly IdTable<EquipmentTypeId, EquipmentTypeInfo, RawEquipmentTypeInfo, MasterDataRoot> _equipmentTypes;
        public ITable<EquipmentTypeId, EquipmentTypeInfo> EquipmentTypes => _equipmentTypes;

        private readonly IdTable<EquipmentInfoId, EquipmentInfo, RawEquipmentInfo, MasterDataRoot> _equipmentInfos;
        public ITable<EquipmentInfoId, EquipmentInfo> EquipmentInfos => _equipmentInfos;

        private readonly IdTable<UseItemId, UseItemInfo, RawUseItem, MasterDataRoot> _useItems;
        public ITable<UseItemId, UseItemInfo> UseItems => _useItems;

        private readonly IdTable<MapAreaId, MapAreaInfo, RawMapArea, MasterDataRoot> _mapAreas;
        public ITable<MapAreaId, MapAreaInfo> MapAreas => _mapAreas;

        private readonly IdTable<MapId, MapInfo, RawMapInfo, MasterDataRoot> _mapInfos;
        public ITable<MapId, MapInfo> MapInfos => _mapInfos;

        private readonly IdTable<ExpeditionId, ExpeditionInfo, RawExpeditionInfo, MasterDataRoot> _expeditions;
        public ITable<ExpeditionId, ExpeditionInfo> Expeditions => _expeditions;
    }
}
