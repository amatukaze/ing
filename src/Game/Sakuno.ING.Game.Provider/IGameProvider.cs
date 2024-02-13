using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Events.MasterData;

namespace Sakuno.ING.Game.Provider;

public interface IGameProvider
{
    IObservable<IMasterDataUpdated> MasterDataUpdated { get; }

    IObservable<IReadOnlyList<IShipUpdated>> ShipsUpdated { get; }
    IObservable<IReadOnlyList<IShipUpdated>> PartialShipsUpdated { get; }

    IObservable<IReadOnlyList<ISlotItemUpdated>> SlotItemsUpdated { get; }
    IObservable<IReadOnlyList<ISlotItemUpdated>> PartialSlotItemsUpdated { get; }

    IObservable<IReadOnlyList<IFleetUpdated>> FleetsUpdated { get; }
    IObservable<IReadOnlyList<IFleetUpdated>> PartialFleetsUpdated { get; }

    IObservable<IReadOnlyList<IConstructionDockUpdated>> ConstructionDocksUpdated { get; }
    IObservable<IReadOnlyList<IRepairDockUpdated>> RepairDocksUpdated { get; }

    IObservable<IReadOnlyList<IUseItemUpdated>> UseItemsUpdated { get; }

    IObservable<IReadOnlyList<IUnequippedSlotItemsUpdated>> UnequippedSlotItemsUpdated { get; }
}
