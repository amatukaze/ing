using System;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    public class MasterDataRoot
    {
        internal MasterDataRoot(IGameProvider listener)
        {
            _shipInfos = new IdTable<ShipInfoId, ShipInfo, IRawShipInfo, MasterDataRoot>(this);
            _shipTypes = new IdTable<ShipTypeId, ShipTypeInfo, IRawShipTypeInfo, MasterDataRoot>(this);
            _equipmentTypes = new IdTable<EquipmentTypeId, EquipmentTypeInfo, IRawEquipmentTypeInfo, MasterDataRoot>(this);
            _equipmentInfos = new IdTable<EquipmentInfoId, EquipmentInfo, IRawEquipmentInfo, MasterDataRoot>(this);
            _useItems = new IdTable<UseItemId, UseItemInfo, IRawUseItem, MasterDataRoot>(this);
            _mapAreas = new IdTable<MapAreaId, MapAreaInfo, IRawMapArea, MasterDataRoot>(this);
            _mapInfos = new IdTable<MapId, MapInfo, IRawMapInfo, MasterDataRoot>(this);
            _expeditions = new IdTable<ExpeditionId, ExpeditionInfo, IRawExpeditionInfo, MasterDataRoot>(this);

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

        private readonly IdTable<ShipInfoId, ShipInfo, IRawShipInfo, MasterDataRoot> _shipInfos;
        public ITable<ShipInfoId, ShipInfo> ShipInfos => _shipInfos;

        private readonly IdTable<ShipTypeId, ShipTypeInfo, IRawShipTypeInfo, MasterDataRoot> _shipTypes;
        public ITable<ShipTypeId, ShipTypeInfo> ShipTypes => _shipTypes;

        private readonly IdTable<EquipmentTypeId, EquipmentTypeInfo, IRawEquipmentTypeInfo, MasterDataRoot> _equipmentTypes;
        public ITable<EquipmentTypeId, EquipmentTypeInfo> EquipmentTypes => _equipmentTypes;

        private readonly IdTable<EquipmentInfoId, EquipmentInfo, IRawEquipmentInfo, MasterDataRoot> _equipmentInfos;
        public ITable<EquipmentInfoId, EquipmentInfo> EquipmentInfos => _equipmentInfos;

        private readonly IdTable<UseItemId, UseItemInfo, IRawUseItem, MasterDataRoot> _useItems;
        public ITable<UseItemId, UseItemInfo> UseItems => _useItems;

        private readonly IdTable<MapAreaId, MapAreaInfo, IRawMapArea, MasterDataRoot> _mapAreas;
        public ITable<MapAreaId, MapAreaInfo> MapAreas => _mapAreas;

        private readonly IdTable<MapId, MapInfo, IRawMapInfo, MasterDataRoot> _mapInfos;
        public ITable<MapId, MapInfo> MapInfos => _mapInfos;

        private readonly IdTable<ExpeditionId, ExpeditionInfo, IRawExpeditionInfo, MasterDataRoot> _expeditions;
        public ITable<ExpeditionId, ExpeditionInfo> Expeditions => _expeditions;
    }
}
