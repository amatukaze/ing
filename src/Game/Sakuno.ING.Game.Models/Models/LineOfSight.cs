using System;

namespace Sakuno.ING.Game.Models
{
    public readonly struct LineOfSight : IEquatable<LineOfSight>
    {
        public LineOfSight(double multiplied, double offset)
        {
            Multiplied = multiplied;
            Offset = offset;
        }

        public double Multiplied { get; }
        public double Offset { get; }
        public double Multiplier1 => Offset + Multiplied;
        public double Multiplier3 => Offset + 3 * Multiplied;
        public double Multiplier4 => Offset + 4 * Multiplied;

        public override bool Equals(object obj) => obj is LineOfSight && Equals((LineOfSight)obj);
        public bool Equals(LineOfSight other) => Multiplied == other.Multiplied && Offset == other.Offset;

        public override int GetHashCode()
        {
            var hashCode = 1040186022;
            hashCode = hashCode * -1521134295 + Multiplied.GetHashCode();
            hashCode = hashCode * -1521134295 + Offset.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(LineOfSight left, LineOfSight right) => left.Equals(right);
        public static bool operator !=(LineOfSight left, LineOfSight right) => !(left == right);

        public static LineOfSight operator +(LineOfSight left, LineOfSight right)
            => new LineOfSight(left.Multiplied + right.Multiplied, left.Offset + right.Offset);

        public static LineOfSight operator +(LineOfSight left, double right)
            => new LineOfSight(left.Multiplied, left.Offset + right);
    }
}
