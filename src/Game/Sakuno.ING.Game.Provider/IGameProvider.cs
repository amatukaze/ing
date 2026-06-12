using System.Reactive;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Events.MasterData;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Provider;

public interface IGameProvider
{
    IObservable<Unit> Committed { get; }

    IObservable<IMasterDataUpdated> MasterDataUpdated { get; }

    IObservable<AdmiralId> AdmiralIdUpdated { get; }
    IObservable<IAdmiralUpdated> AdmiralUpdated { get; }

    IObservable<IMaterialsUpdated> MaterialsUpdated { get; }

    IObservable<IReadOnlyList<IShipUpdated>> ShipsUpdated { get; }
    IObservable<IReadOnlyList<IShipUpdated>> PartialShipsUpdated { get; }

    IObservable<IReadOnlyList<ISlotItemUpdated>> SlotItemsUpdated { get; }
    IObservable<IReadOnlyList<ISlotItemUpdated>> PartialSlotItemsUpdated { get; }

    IObservable<IReadOnlyList<IFleetUpdated>> FleetsUpdated { get; }
    IObservable<IReadOnlyList<IFleetUpdated>> PartialFleetsUpdated { get; }
    IObservable<IFleetPatched> FleetPatched { get; }

    IObservable<IReadOnlyList<IConstructionDockUpdated>> ConstructionDocksUpdated { get; }
    IObservable<IReadOnlyList<IRepairDockUpdated>> RepairDocksUpdated { get; }

    IObservable<IReadOnlyList<IUseItemUpdated>> UseItemsUpdated { get; }

    IObservable<IReadOnlyList<IUnequippedSlotItemsUpdated>> UnequippedSlotItemsUpdated { get; }

    IObservable<IReadOnlyList<IAirForceGroupUpdated>> AirForceGroupsUpdated { get; }

    IObservable<IReadOnlyList<IQuestUpdated>> PartialQuestsUpdated { get; }
}
