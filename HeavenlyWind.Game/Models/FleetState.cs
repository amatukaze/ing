using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    [Flags]
    public enum FleetState
    {
        None,
        Idle = 1,
        Expedition = 1 << 1,
        Sortie = 1 << 2,
        Unsupplied = 1 << 3,
        Repairing = 1 << 4,
        HeavilyDamaged = 1 << 5,
        AnchorageRepair = 1 << 6,
        HighMorale = 1 << 7,
        ModerateTired = 1 << 8,
        SeriouslyTired = 1 << 9,
    }
}
