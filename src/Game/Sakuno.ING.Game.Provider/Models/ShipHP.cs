using System;

namespace Sakuno.ING.Game.Models
{
    public readonly struct ShipHP : IEquatable<ShipHP>
    {
        private readonly ClampedValue _value;

        public int Current => _value.Current;
        public int Max => _value.Max;
        public ShipDamageState DamageState
        {
            get
            {
                if (Current == Max)
                    return ShipDamageState.FullyHealthy;
                if (Current * 4 > Max * 3)
                    return ShipDamageState.Healthy;
                if (Current * 4 > Max * 2)
                    return ShipDamageState.LightlyDamaged;
                if (Current * 4 > Max)
                    return ShipDamageState.ModeratelyDamaged;
                if (Current > 0)
                    return ShipDamageState.HeavilyDamaged;

                return ShipDamageState.Sunk;
            }
        }

        public ShipHP(ClampedValue value) => this._value = value;
        public ShipHP(int current, int max) : this(new ClampedValue(current, max)) { }

        public static ShipHP operator +(ShipHP hp, int value) => new ShipHP(hp.Current + value, hp.Max);
        public static ShipHP operator -(ShipHP hp, int value) => new ShipHP(hp.Current - value, hp.Max);

        public static bool operator ==(ShipHP left, ShipHP right) => left._value == right._value;
        public static bool operator !=(ShipHP left, ShipHP right) => left._value != right._value;

        public override bool Equals(object obj) => obj is ShipHP hp && this == hp;
        public bool Equals(ShipHP other) => this == other;
        public override int GetHashCode() => _value.GetHashCode();

        public static implicit operator ShipHP((int current, int max) tuple)  =>
            new ShipHP(tuple.current, tuple.max);

        public void Deconstruct(out int current, out int max)
        {
            current = Current;
            max = Max;
        }

        public override string ToString() => $"{Current}/{Max}";
    }
}
