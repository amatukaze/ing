using System;
using Sakuno.KanColle.Amatsukaze.Game.Events;
using Sakuno.KanColle.Amatsukaze.Game.Models.MasterData;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class MasterDataRoot : ITableProvider
    {
        internal MasterDataRoot(GameListener listener)
        {
            _shipInfos = new IdTable<ShipInfo, IRawShipInfo>(this);
            _shipTypes = new IdTable<ShipTypeInfo, IRawShipTypeInfo>(this);
            _equipmentTypes = new IdTable<EquipmentTypeInfo, IRawEquipmentTypeInfo>(this);
            _equipmentInfos = new IdTable<EquipmentInfo, IRawEquipmentInfo>(this);
            _mapAreas = new IdTable<MapAreaInfo, IRawMapArea>(this);
            _mapInfos = new IdTable<MapInfo, IRawMapInfo>(this);
            _expeditions = new IdTable<ExpeditionInfo, IRawExpeditionInfo>(this);

            listener.MasterDataUpdated.Received += OnMasterDataUpdated;
        }

        private void OnMasterDataUpdated(MasterDataUpdate obj)
        {
            _shipTypes.BatchUpdate(obj.ShipTypes);
            _shipInfos.BatchUpdate(obj.ShipInfos);
            _equipmentTypes.BatchUpdate(obj.EquipmentTypes);
            _equipmentInfos.BatchUpdate(obj.EquipmentInfos);
            _mapAreas.BatchUpdate(obj.MapAreas);
            _mapInfos.BatchUpdate(obj.Maps);
            _expeditions.BatchUpdate(obj.Expeditions);
        }

        private readonly IdTable<ShipInfo, IRawShipInfo> _shipInfos;
        public ITable<ShipInfo> ShipInfos => _shipInfos;

        private readonly IdTable<ShipTypeInfo, IRawShipTypeInfo> _shipTypes;
        public ITable<ShipTypeInfo> ShipTypes => _shipTypes;

        private readonly IdTable<EquipmentTypeInfo, IRawEquipmentTypeInfo> _equipmentTypes;
        public ITable<EquipmentTypeInfo> EquipmentTypes => _equipmentTypes;

        private readonly IdTable<EquipmentInfo, IRawEquipmentInfo> _equipmentInfos;
        public ITable<EquipmentInfo> EquipmentInfos => _equipmentInfos;

        private readonly IdTable<MapAreaInfo, IRawMapArea> _mapAreas;
        public ITable<MapAreaInfo> MapAreas => _mapAreas;

        private readonly IdTable<MapInfo, IRawMapInfo> _mapInfos;
        public ITable<MapInfo> MapInfos => _mapInfos;

        private readonly IdTable<ExpeditionInfo, IRawExpeditionInfo> _expeditions;
        public ITable<ExpeditionInfo> Expeditions => _expeditions;

        public ITable<T> GetTable<T>()
        {
            var type = typeof(T);

            if (type == typeof(ShipInfo))
                return (ITable<T>)ShipInfos;

            if (type == typeof(ShipTypeInfo))
                return (ITable<T>)ShipTypes;

            if (type == typeof(EquipmentTypeInfo))
                return (ITable<T>)EquipmentTypes;

            if (type == typeof(EquipmentInfo))
                return (ITable<T>)EquipmentInfos;

            if (type == typeof(MapAreaInfo))
                return (ITable<T>)MapAreas;

            if (type == typeof(MapInfo))
                return (ITable<T>)MapInfos;

            if (type == typeof(ExpeditionInfo))
                return (ITable<T>)Expeditions;

            throw new ArgumentException($"Unknown type {type}.");
        }
    }
}
