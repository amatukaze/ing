namespace Sakuno.ING.Game.Models.Battle
{
    public interface IRawHit
    {
        int TargetIndex { get; }
        int Damage { get; }
        bool IsCritical { get; }
        bool IsProtection { get; }
    }
}
