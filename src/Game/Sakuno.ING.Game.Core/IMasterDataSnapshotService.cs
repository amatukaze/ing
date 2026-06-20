using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game;

public interface IMasterDataSnapshotService
{
    ITableSnapshot<ShipInfo, ShipInfoId> Ships { get; }
    ITableSnapshot<ShipTypeInfo, ShipTypeId> ShipTypes { get; }
    ITableSnapshot<SlotItemInfo, SlotItemInfoId> SlotItems { get; }
    ITableSnapshot<SlotItemTypeInfo, SlotItemTypeId> SlotItemTypes { get; }
    ITableSnapshot<UseItemInfo, UseItemId> UseItems { get; }
    ITableSnapshot<MapAreaInfo, MapAreaId> MapAreas { get; }
    ITableSnapshot<MapInfo, MapId> Maps { get; }
    ITableSnapshot<ExpeditionInfo, ExpeditionId> Expeditions { get; }
}
