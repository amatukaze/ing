using System;
using static System.Math;

namespace Sakuno.ING.Game.Models
{
    public readonly struct AirFightPower : IEquatable<AirFightPower>
    {
        public AirFightPower(double raw, double minimum, double maximum)
        {
            Raw = raw;
            Minimum = minimum;
            Maximum = maximum;
        }

        public double Raw { get; }
        public double Minimum { get; }
        public double Maximum { get; }

        public static AirFightPower operator +(AirFightPower left, AirFightPower right)
            => new AirFightPower
            (
                Floor(left.Raw) + Floor(right.Raw),
                Floor(left.Minimum) + Floor(right.Minimum),
                Floor(left.Maximum) + Floor(right.Maximum)
            );

        public override bool Equals(object obj) => obj is AirFightPower && Equals((AirFightPower)obj);
        public bool Equals(AirFightPower other) => Raw == other.Raw && Minimum == other.Minimum && Maximum == other.Maximum;

        public override int GetHashCode() => HashCode.Combine(Raw, Minimum, Maximum);

        public static bool operator ==(AirFightPower left, AirFightPower right)
            => left.Equals(right);
        public static bool operator !=(AirFightPower left, AirFightPower right)
            => !left.Equals(right);
    }
}
