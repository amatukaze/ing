using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public interface IRawRepairingDock : IIdentifiable
    {
        RepairingDockState State { get; }
        int RepairingShipId { get; }
        DateTimeOffset CompletionTime { get; }
        Materials Consumption { get; }
    }

    public enum RepairingDockState
    {
        Locked = -1,
        Empty = 0,
        Repairing = 1,
    }
}
