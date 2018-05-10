using System;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    public class MasterDataRoot
    {
        internal MasterDataRoot(IGameProvider listener)
        {
            _shipInfos = new IdTable<int, ShipInfo, IRawShipInfo, MasterDataRoot>(this);
            _shipTypes = new IdTable<int, ShipTypeInfo, IRawShipTypeInfo, MasterDataRoot>(this);
            _equipmentTypes = new IdTable<int, EquipmentTypeInfo, IRawEquipmentTypeInfo, MasterDataRoot>(this);
            _equipmentInfos = new IdTable<int, EquipmentInfo, IRawEquipmentInfo, MasterDataRoot>(this);
            _useItems = new IdTable<int, UseItemInfo, IRawUseItem, MasterDataRoot>(this);
            _mapAreas = new IdTable<int, MapAreaInfo, IRawMapArea, MasterDataRoot>(this);
            _mapInfos = new IdTable<int, MapInfo, IRawMapInfo, MasterDataRoot>(this);
            _expeditions = new IdTable<int, ExpeditionInfo, IRawExpeditionInfo, MasterDataRoot>(this);

            listener.MasterDataUpdated += OnMasterDataUpdated;
        }

        private void OnMasterDataUpdated(DateTimeOffset timeStamp, MasterDataUpdate message)
        {
            _shipTypes.BatchUpdate(message.ShipTypes);
            _shipInfos.BatchUpdate(message.ShipInfos);
            _equipmentTypes.BatchUpdate(message.EquipmentTypes);
            _equipmentInfos.BatchUpdate(message.EquipmentInfos);
            _useItems.BatchUpdate(message.UseItems);
            _mapAreas.BatchUpdate(message.MapAreas);
            _mapInfos.BatchUpdate(message.Maps);
            _expeditions.BatchUpdate(message.Expeditions);
        }

        private readonly IdTable<int, ShipInfo, IRawShipInfo, MasterDataRoot> _shipInfos;
        public ITable<int, ShipInfo> ShipInfos => _shipInfos;

        private readonly IdTable<int, ShipTypeInfo, IRawShipTypeInfo, MasterDataRoot> _shipTypes;
        public ITable<int, ShipTypeInfo> ShipTypes => _shipTypes;

        private readonly IdTable<int, EquipmentTypeInfo, IRawEquipmentTypeInfo, MasterDataRoot> _equipmentTypes;
        public ITable<int, EquipmentTypeInfo> EquipmentTypes => _equipmentTypes;

        private readonly IdTable<int, EquipmentInfo, IRawEquipmentInfo, MasterDataRoot> _equipmentInfos;
        public ITable<int, EquipmentInfo> EquipmentInfos => _equipmentInfos;

        private readonly IdTable<int, UseItemInfo, IRawUseItem, MasterDataRoot> _useItems;
        public ITable<int, UseItemInfo> UseItems => _useItems;

        private readonly IdTable<int, MapAreaInfo, IRawMapArea, MasterDataRoot> _mapAreas;
        public ITable<int, MapAreaInfo> MapAreas => _mapAreas;

        private readonly IdTable<int, MapInfo, IRawMapInfo, MasterDataRoot> _mapInfos;
        public ITable<int, MapInfo> MapInfos => _mapInfos;

        private readonly IdTable<int, ExpeditionInfo, IRawExpeditionInfo, MasterDataRoot> _expeditions;
        public ITable<int, ExpeditionInfo> Expeditions => _expeditions;
    }
}
