namespace Sakuno.ING.Game.Models.Battle
{
    public readonly struct RawHit
    {
        public RawHit(int targetIndex, int damage, bool isCritical, bool isProtection)
        {
            TargetIndex = targetIndex;
            Damage = damage;
            IsCritical = isCritical;
            IsProtection = isProtection;
        }

        public int TargetIndex { get; }
        public int Damage { get; }
        public bool IsCritical { get; }
        public bool IsProtection { get; }
    }
}
