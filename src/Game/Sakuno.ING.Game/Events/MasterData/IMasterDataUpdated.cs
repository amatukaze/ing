namespace Sakuno.ING.Game.Events.MasterData;

public interface IMasterDataUpdated
{
    IReadOnlyList<IShipInfoUpdated> Ships { get; }
    IReadOnlyList<IShipTypeInfoUpdated> ShipTypes { get; }
    IReadOnlyList<ISlotItemInfoUpdated> SlotItems { get; }
    IReadOnlyList<ISlotItemTypeInfoUpdated> SlotItemTypes { get; }
    IReadOnlyList<IUseItemInfoUpdated> UseItems { get; }
    IReadOnlyList<IMapAreaInfoUpdated> MapAreas { get; }
    IReadOnlyList<IMapInfoUpdated> Maps { get; }
    IReadOnlyList<IExpeditionInfoUpdated> Expeditions { get; }
}
