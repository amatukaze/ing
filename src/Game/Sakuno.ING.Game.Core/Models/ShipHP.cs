namespace Sakuno.ING.Game.Models;

public readonly record struct ShipHP(int Current, int Max)
{
    public double Percentage => Current / (double)Max;

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

    public static implicit operator ShipHP((int current, int max) tuple)  =>
        new ShipHP(tuple.current, tuple.max);

    public override string ToString() => $"{Current}/{Max}";
}
