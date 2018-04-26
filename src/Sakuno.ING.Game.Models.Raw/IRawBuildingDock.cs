using System;

namespace Sakuno.ING.Game.Models
{
    public interface IRawBuildingDock : IIdentifiable
    {
        DateTimeOffset CompletionTime { get; }
        BuildingDockState State { get; }
        Materials Consumption { get; }
        int BuiltShipId { get; }
        bool IsLSC { get; }
    }

    public enum BuildingDockState
    {
        Locked = -1,
        Empty = 0,
        Building = 2,
        BuildCompleted = 3,
    }
}
