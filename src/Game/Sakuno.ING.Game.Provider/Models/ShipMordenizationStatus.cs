using System;

namespace Sakuno.ING.Game.Models
{
    public struct ShipMordenizationStatus
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public int Improved { get; set; }
        public int Current => Min + Improved;
        public int Remaining => Math.Max(Max - Current, 0);
        public int Displaying { get; set; }

        public ShipMordenizationStatus(int value)
        {
            Min = Max = Displaying = value;
            Improved = 0;
        }
    }
}
