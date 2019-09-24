using System;

namespace Sakuno.ING.Game.Models
{
    public readonly struct ClampedValue : IEquatable<ClampedValue>
    {
        public ClampedValue(int current, int max)
        {
            Current =
                current > max ? max :
                current < 0 ? 0 :
                current;
            Max = max;
        }

        public int Current { get; }
        public int Max { get; }
        public double Percentage => (double)Current / Max;
        public int Shortage => Max - Current;
        public bool IsMaximum => Current == Max;

        public static ClampedValue operator +(ClampedValue clamped, int value) => new ClampedValue(Math.Min(clamped.Max, clamped.Current + value), clamped.Max);
        public static ClampedValue operator -(ClampedValue clamped, int value) => new ClampedValue(Math.Max(0, clamped.Current - value), clamped.Max);

        public static bool operator ==(ClampedValue left, ClampedValue right)
            => left.Current == right.Current && left.Max == right.Max;
        public static bool operator !=(ClampedValue left, ClampedValue right)
            => !(left == right);

        public override bool Equals(object obj) => obj is ClampedValue v && this == v;
        public bool Equals(ClampedValue other) => this == other;
        public override int GetHashCode() => HashCode.Combine(Current, Max);

        public static implicit operator ClampedValue((int current, int max) tuple)
            => new ClampedValue(tuple.current, tuple.max);

        public static ClampedValue FromShortage(int max, int shortage)
            => new ClampedValue(max - shortage, max);

        public void Deconstruct(out int current, out int max)
        {
            current = Current;
            max = Max;
        }

        public override string ToString() => $"{Current}/{Max}";
    }
}
