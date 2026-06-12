using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game;

public interface IMasterDataService
{
    ITable<ShipInfo, ShipInfoId> Ships { get; }
    ITable<ShipTypeInfo, ShipTypeId> ShipTypes { get; }
    ITable<SlotItemInfo, SlotItemInfoId> SlotItems { get; }
    ITable<SlotItemTypeInfo, SlotItemTypeId> SlotItemTypes { get; }
    ITable<UseItemInfo, UseItemId> UseItems { get; }
    ITable<MapAreaInfo, MapAreaId> MapAreas { get; }
    ITable<MapInfo, MapId> Maps { get; }
    ITable<ExpeditionInfo, ExpeditionId> Expeditions { get; }
}
