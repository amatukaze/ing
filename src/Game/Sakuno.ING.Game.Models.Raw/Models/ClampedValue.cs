namespace Sakuno.ING.Game.Models
{
    public readonly struct ClampedValue
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
        public double Precentage => (double)Current / Max;
        public int Shortage => Max - Current;

        public static ClampedValue operator +(ClampedValue clamped, int value) => new ClampedValue(clamped.Current + value, clamped.Max);
        public static ClampedValue operator -(ClampedValue clamped, int value) => new ClampedValue(clamped.Current - value, clamped.Max);

        public static bool operator ==(ClampedValue left, ClampedValue right)
            => left.Current == right.Current && left.Max == right.Max;
        public static bool operator !=(ClampedValue left, ClampedValue right)
            => !(left == right);

        public override bool Equals(object obj) => obj is ClampedValue v && this == v;
        public override int GetHashCode() => Current.GetHashCode() ^ Max.GetHashCode();

        public static implicit operator ClampedValue((int current, int max) tuple)
            => new ClampedValue(tuple.current, tuple.max);

        public void Deconstruct(out int current, out int max)
        {
            current = Current;
            max = Max;
        }
    }
}
