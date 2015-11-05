namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public enum ShipState
    {
        None,
        Repairing = 1,
        Retreated = 1 << 1,
        HeavilyDamaged = 1 << 2,
    }
}
