using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public abstract class RepairingDock : ManualNotifyObject, IIdentifiable
    {
        public int Id { get; protected set; }
        public DateTimeOffset CompletionTime { get; protected set; }
        public RepairingDockState State { get; protected set; }
        public Ship Ship { get; protected set; }
        public int FuelConsumption { get; protected set; }
        public int SteelConsumption { get; protected set; }
    }

    public enum RepairingDockState
    {
        Locked = -1,
        Empty = 0,
        Repairing = 1,
    }
}
