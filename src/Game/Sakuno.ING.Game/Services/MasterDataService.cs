using System.Reactive.Linq;
using Sakuno.ING.Game.Events.MasterData;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Game.Provider;

namespace Sakuno.ING.Game.Services;

[RegisterSingleton(Registration = RegistrationStrategy.SelfWithProxyFactory)]
public class MasterDataService : IMasterDataService
{
    public ITable<ShipInfo, ShipInfoId> Ships { get; }
    public ITable<ShipTypeInfo, ShipTypeId> ShipTypes { get; }
    public ITable<SlotItemInfo, SlotItemInfoId> SlotItems { get; }
    public ITable<SlotItemTypeInfo, SlotItemTypeId> SlotItemTypes { get; }
    public ITable<UseItemInfo, UseItemId> UseItems { get; }
    public ITable<MapAreaInfo, MapAreaId> MapAreas { get; }
    public ITable<MapInfo, MapId> Maps { get; }
    public ITable<ExpeditionInfo, ExpeditionId> Expeditions { get; }

    public MasterDataService(IGameProvider gameProvider)
    {
        Ships = new Table<ShipInfo, ShipInfoId, IShipInfoUpdated>(
            gameProvider.MasterDataUpdated.Select(masterData => masterData.Ships));
        ShipTypes = new Table<ShipTypeInfo, ShipTypeId, IShipTypeInfoUpdated>(
            gameProvider.MasterDataUpdated.Select(masterData => masterData.ShipTypes));
        SlotItems = new Table<SlotItemInfo, SlotItemInfoId, ISlotItemInfoUpdated>(
            gameProvider.MasterDataUpdated.Select(masterData => masterData.SlotItems));
        SlotItemTypes = new Table<SlotItemTypeInfo, SlotItemTypeId, ISlotItemTypeInfoUpdated>(
            gameProvider.MasterDataUpdated.Select(masterData => masterData.SlotItemTypes));
        UseItems = new Table<UseItemInfo, UseItemId, IUseItemInfoUpdated>(
            gameProvider.MasterDataUpdated.Select(masterData => masterData.UseItems));
        MapAreas = new Table<MapAreaInfo, MapAreaId, IMapAreaInfoUpdated>(
            gameProvider.MasterDataUpdated.Select(masterData => masterData.MapAreas));
        Maps = new Table<MapInfo, MapId, IMapInfoUpdated>(
            gameProvider.MasterDataUpdated.Select(masterData => masterData.Maps));
        Expeditions = new Table<ExpeditionInfo, ExpeditionId, IExpeditionInfoUpdated>(
            gameProvider.MasterDataUpdated.Select(masterData => masterData.Expeditions));
    }
}
