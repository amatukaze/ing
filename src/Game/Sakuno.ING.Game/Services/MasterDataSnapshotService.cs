using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Services;

[RegisterSingleton(Registration = RegistrationStrategy.ImplementedInterfaces)]
internal class MasterDataSnapshotService(IMasterDataService service) : IMasterDataSnapshotService
{
    public ITableSnapshot<ShipInfo, ShipInfoId> Ships => service.Ships.Snapshot;
    public ITableSnapshot<ShipTypeInfo, ShipTypeId> ShipTypes => service.ShipTypes.Snapshot;
    public ITableSnapshot<SlotItemInfo, SlotItemInfoId> SlotItems => service.SlotItems.Snapshot;
    public ITableSnapshot<SlotItemTypeInfo, SlotItemTypeId> SlotItemTypes => service.SlotItemTypes.Snapshot;
    public ITableSnapshot<UseItemInfo, UseItemId> UseItems => service.UseItems.Snapshot;
    public ITableSnapshot<MapAreaInfo, MapAreaId> MapAreas => service.MapAreas.Snapshot;
    public ITableSnapshot<MapInfo, MapId> Maps => service.Maps.Snapshot;
    public ITableSnapshot<ExpeditionInfo, ExpeditionId> Expeditions => service.Expeditions.Snapshot;
}
