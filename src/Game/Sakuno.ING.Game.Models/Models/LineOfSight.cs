using System;
using System.ComponentModel;
using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Game.Models
{
    public readonly struct LineOfSight : IEquatable<LineOfSight>
    {
        public LineOfSight(double multiplied, double baseline, int admiralLevel = 0)
        {
            Multiplied = multiplied;
            Baseline = baseline;
            AdmiralLevel = admiralLevel;
        }

        public double Multiplied { get; }
        public double Baseline { get; }
        public int AdmiralLevel { get; }
        public double OffsetFromLevel => -Math.Ceiling(0.4 * AdmiralLevel);
        public LineOfSight AtMaxLevel
            => new LineOfSight(Multiplied, Baseline, KnownLeveling.MaxAdmiralLevel);
        public double MultiplyWith(int factor)
            => Baseline + factor * Multiplied + OffsetFromLevel;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string MultiplyAndGetString(int factor)
            => MultiplyWith(factor).ToString("F2");

        public override bool Equals(object obj)
            => obj is LineOfSight other
            && Equals(other);
        public bool Equals(LineOfSight other)
            => Multiplied == other.Multiplied
            && Baseline == other.Baseline
            && AdmiralLevel == other.AdmiralLevel;

        public override int GetHashCode() => HashCode.Combine(Multiplied, Baseline, AdmiralLevel);

        public static bool operator ==(LineOfSight left, LineOfSight right) => left.Equals(right);
        public static bool operator !=(LineOfSight left, LineOfSight right) => !(left == right);

        public static LineOfSight operator +(LineOfSight left, LineOfSight right)
            => new LineOfSight(left.Multiplied + right.Multiplied, left.Baseline + right.Baseline);
    }
}
