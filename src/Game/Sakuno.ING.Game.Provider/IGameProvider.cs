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
    IObservable<IShipPatched> ShipPatched { get; }
    IObservable<IReadOnlyList<ShipId>> ShipsRemoved { get; }

    IObservable<IReadOnlyList<ISlotItemUpdated>> SlotItemsUpdated { get; }
    IObservable<IReadOnlyList<ISlotItemUpdated>> PartialSlotItemsUpdated { get; }
    IObservable<ISlotItemPatched> SlotItemPatched { get; }
    IObservable<IReadOnlyList<SlotItemId>> SlotItemsRemoved { get; }

    IObservable<IReadOnlyList<IFleetUpdated>> FleetsUpdated { get; }
    IObservable<IReadOnlyList<IFleetUpdated>> PartialFleetsUpdated { get; }
    IObservable<IFleetPatched> FleetPatched { get; }

    IObservable<IReadOnlyList<IConstructionDockUpdated>> ConstructionDocksUpdated { get; }
    IObservable<IConstructionDockPatched> ConstructionDockPatched { get; }
    IObservable<IReadOnlyList<IRepairDockUpdated>> RepairDocksUpdated { get; }
    IObservable<IRepairDockPatched> RepairDockPatched { get; }

    IObservable<IReadOnlyList<IUseItemUpdated>> UseItemsUpdated { get; }

    IObservable<IReadOnlyList<IUnequippedSlotItemsUpdated>> UnequippedSlotItemsUpdated { get; }

    IObservable<IReadOnlyList<IAirForceGroupUpdated>> AirForceGroupsUpdated { get; }
    IObservable<IAirForceGroupPatched> AirForceGroupPatched { get; }

    IObservable<IReadOnlyList<IQuestUpdated>> PartialQuestsUpdated { get; }
}
