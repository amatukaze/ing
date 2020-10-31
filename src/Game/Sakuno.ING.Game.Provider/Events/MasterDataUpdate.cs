using Sakuno.ING.Game.Models.MasterData;
using System.Collections.Generic;

namespace Sakuno.ING.Game.Events
{
    public sealed class MasterDataUpdate
    {
        public IReadOnlyCollection<RawShipInfo> ShipInfos { get; }
        public IReadOnlyCollection<RawShipTypeInfo> ShipTypes { get; }
        public IReadOnlyCollection<RawSlotItemTypeInfo> SlotItemTypes { get; }
        public IReadOnlyCollection<RawSlotItemInfo> SlotItemInfos { get; }
        public IReadOnlyCollection<RawUseItem> UseItems { get; }
        public IReadOnlyCollection<RawMapArea> MapAreas { get; }
        public IReadOnlyCollection<RawMapInfo> Maps { get; }
        public IReadOnlyCollection<RawExpeditionInfo> Expeditions { get; }

        public MasterDataUpdate
        (
            IReadOnlyCollection<RawShipInfo> shipInfos,
            IReadOnlyCollection<RawShipTypeInfo> shipTypes,
            IReadOnlyCollection<RawSlotItemInfo> slotItemInfos,
            IReadOnlyCollection<RawSlotItemTypeInfo> slotItemTypes,
            IReadOnlyCollection<RawUseItem> useItems,
            IReadOnlyCollection<RawMapArea> mapAreas,
            IReadOnlyCollection<RawMapInfo> maps,
            IReadOnlyCollection<RawExpeditionInfo> expeditions
        )
        {
            ShipInfos = shipInfos;
            ShipTypes = shipTypes;
            SlotItemTypes = slotItemTypes;
            SlotItemInfos = slotItemInfos;
            UseItems = useItems;
            MapAreas = mapAreas;
            Maps = maps;
            Expeditions = expeditions;
        }
    }
}
