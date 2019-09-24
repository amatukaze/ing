using System;

namespace Sakuno.ING.Game.Models
{
    public readonly struct ShipHP : IEquatable<ShipHP>
    {
        private readonly ClampedValue value;
        public ShipHP(ClampedValue value) => this.value = value;
        public ShipHP(int current, int max) : this(new ClampedValue(current, max)) { }

        public int Current => value.Current;
        public int Max => value.Max;
        public ShipDamageState DamageState
        {
            get
            {
                if (Current == Max) return ShipDamageState.FullyHealthy;
                if (Current * 4 > Max * 3) return ShipDamageState.Healthy;
                if (Current * 4 > Max * 2) return ShipDamageState.LightlyDamaged;
                if (Current * 4 > Max) return ShipDamageState.ModeratelyDamaged;
                if (Current > 0) return ShipDamageState.HeavilyDamaged;
                return ShipDamageState.Sunk;
            }
        }

        public static ShipHP operator +(ShipHP hp, int value) => new ShipHP(hp.Current + value, hp.Max);
        public static ShipHP operator -(ShipHP hp, int value) => new ShipHP(hp.Current - value, hp.Max);

        public static bool operator ==(ShipHP left, ShipHP right)
            => left.value == right.value;
        public static bool operator !=(ShipHP left, ShipHP right)
            => left.value != right.value;

        public override bool Equals(object obj) => obj is ShipHP v && this == v;
        public bool Equals(ShipHP other) => this == other;
        public override int GetHashCode() => value.GetHashCode();

        public static implicit operator ShipHP((int current, int max) tuple)
            => new ShipHP(tuple.current, tuple.max);

        public void Deconstruct(out int current, out int max)
        {
            current = Current;
            max = Max;
        }

        public override string ToString() => $"{Current}/{Max}";
    }

    public enum ShipDamageState
    {
        FullyHealthy = 0,
        Healthy = 1,
        LightlyDamaged = 2,
        ModeratelyDamaged = 3,
        HeavilyDamaged = 4,
        Sunk = 5
    }
}
