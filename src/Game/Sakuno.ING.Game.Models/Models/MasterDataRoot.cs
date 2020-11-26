using Sakuno.ING.Game.Models.MasterData;
using System;
using System.Reactive.Linq;

namespace Sakuno.ING.Game.Models
{
    public sealed class MasterDataRoot
    {
        private readonly IdTable<ShipInfoId, ShipInfo, RawShipInfo, MasterDataRoot> _shipInfos;
        public ITable<ShipInfoId, ShipInfo> ShipInfos => _shipInfos;

        private readonly IdTable<ShipTypeId, ShipTypeInfo, RawShipTypeInfo, MasterDataRoot> _shipTypes;
        public ITable<ShipTypeId, ShipTypeInfo> ShipTypes => _shipTypes;

        private readonly IdTable<SlotItemInfoId, SlotItemInfo, RawSlotItemInfo, MasterDataRoot> _slotItemInfos;
        public ITable<SlotItemInfoId, SlotItemInfo> SlotItemInfos => _slotItemInfos;

        private readonly IdTable<SlotItemTypeId, SlotItemTypeInfo, RawSlotItemTypeInfo, MasterDataRoot> _slotItemTypes;
        public ITable<SlotItemTypeId, SlotItemTypeInfo> SlotItemTypes => _slotItemTypes;

        private readonly IdTable<UseItemId, UseItemInfo, RawUseItem, MasterDataRoot> _useItems;
        public ITable<UseItemId, UseItemInfo> UseItems => _useItems;

        private readonly IdTable<MapAreaId, MapAreaInfo, RawMapArea, MasterDataRoot> _mapAreas;
        public ITable<MapAreaId, MapAreaInfo> MapAreas => _mapAreas;

        private readonly IdTable<MapId, MapInfo, RawMapInfo, MasterDataRoot> _mapInfos;
        public ITable<MapId, MapInfo> MapInfos => _mapInfos;

        private readonly IdTable<ExpeditionId, ExpeditionInfo, RawExpeditionInfo, MasterDataRoot> _expeditions;
        public ITable<ExpeditionId, ExpeditionInfo> Expeditions => _expeditions;

        internal MasterDataRoot(GameProvider provider)
        {
            _shipInfos = new IdTable<ShipInfoId, ShipInfo, RawShipInfo, MasterDataRoot>(this);
            _shipTypes = new IdTable<ShipTypeId, ShipTypeInfo, RawShipTypeInfo, MasterDataRoot>(this);
            _slotItemInfos = new IdTable<SlotItemInfoId, SlotItemInfo, RawSlotItemInfo, MasterDataRoot>(this);
            _slotItemTypes = new IdTable<SlotItemTypeId, SlotItemTypeInfo, RawSlotItemTypeInfo, MasterDataRoot>(this);
            _useItems = new IdTable<UseItemId, UseItemInfo, RawUseItem, MasterDataRoot>(this);
            _mapAreas = new IdTable<MapAreaId, MapAreaInfo, RawMapArea, MasterDataRoot>(this);
            _mapInfos = new IdTable<MapId, MapInfo, RawMapInfo, MasterDataRoot>(this);
            _expeditions = new IdTable<ExpeditionId, ExpeditionInfo, RawExpeditionInfo, MasterDataRoot>(this);

            provider.MasterDataUpdated.Subscribe(message =>
            {
                _shipTypes.BatchUpdate(message.ShipTypes);
                _shipInfos.BatchUpdate(message.ShipInfos);
                _slotItemTypes.BatchUpdate(message.SlotItemTypes);
                _slotItemInfos.BatchUpdate(message.SlotItemInfos);
                _useItems.BatchUpdate(message.UseItems);
                _mapAreas.BatchUpdate(message.MapAreas);
                _mapInfos.BatchUpdate(message.Maps);
                _expeditions.BatchUpdate(message.Expeditions);
            });
        }
    }
}
