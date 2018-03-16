using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public sealed class CombinedFleet
    {
        public CombinedFleet(Fleet mainFleet, Fleet escortFleet, FleetType type)
        {
            MainFleet = mainFleet ?? throw new ArgumentNullException(nameof(mainFleet));
            EscortFleet = escortFleet ?? throw new ArgumentNullException(nameof(escortFleet));
            Type = type;
        }

        public Fleet MainFleet { get; }
        public Fleet EscortFleet { get; }
        public FleetType Type { get; }
    }
}
