using System;

namespace Sakuno.ING.Game.Models
{
    public readonly struct ShipMordenizationStatus : IEquatable<ShipMordenizationStatus>
    {
        public int Min { get; }
        public int Max { get; }
        public int Improved { get; }
        public int Current => Min + Improved;
        public int Remaining => Math.Max(Max - Current, 0);
        public int Displaying { get; }

        public ShipMordenizationStatus(int value)
        {
            Min = Max = Displaying = value;
            Improved = 0;
        }

        public ShipMordenizationStatus(int value, int displaying)
        {
            Min = Max = value;
            Improved = 0;
            Displaying = displaying;
        }

        public ShipMordenizationStatus(int max, int displaying = 0, int improved = 0, int min = 0)
        {
            Min = min;
            Max = max;
            Improved = improved;
            Displaying = displaying;
        }

        public override string ToString() => $"{Displaying} ({Current}+{Displaying - Current})";

        public override bool Equals(object obj) => obj is ShipMordenizationStatus status && Equals(status);
        public bool Equals(ShipMordenizationStatus other)
            => Min == other.Min
            && Max == other.Max
            && Improved == other.Improved
            && Displaying == other.Displaying;

        public override int GetHashCode() => HashCode.Combine(Min, Max, Improved, Displaying);

        public static bool operator ==(ShipMordenizationStatus left, ShipMordenizationStatus right) => left.Equals(right);
        public static bool operator !=(ShipMordenizationStatus left, ShipMordenizationStatus right) => !(left == right);
    }
}
