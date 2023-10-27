namespace Sakuno.ING.Game.Events;

public interface IHomeportDataUpdated
{
    IReadOnlyList<IShipUpdated> Ships { get; }
    IReadOnlyList<IRepairDockUpdated> RepairDocks { get; }
    IReadOnlyList<IFleetUpdated> Fleets { get; }
}
