using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public struct ShipMordenizationStatus
    {
        public int Min { get; }
        public int Max { get; }
        public int Current { get; }
        public int Remaining => Math.Max(Max - Current, 0);

        public ShipMordenizationStatus(int min, int max, int upgraded)
        {
            if (min < 0) throw new ArgumentOutOfRangeException(nameof(min));
            if (max < min) throw new ArgumentOutOfRangeException(nameof(max));
            Min = min;
            Max = max;
            Current = min + upgraded;
        }
    }
}
