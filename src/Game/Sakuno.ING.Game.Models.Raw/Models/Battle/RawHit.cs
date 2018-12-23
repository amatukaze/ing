namespace Sakuno.ING.Game.Models.Battle
{
    public readonly struct RawHit
    {
        public RawHit(int? sourceIndex, int targetIndex, int damage, bool isCritical)
        {
            SourceIndex = sourceIndex;
            TargetIndex = targetIndex;
            Damage = damage;
            IsCritical = isCritical;
        }

        public int? SourceIndex { get; }
        public int TargetIndex { get; }
        public int Damage { get; }
        public bool IsCritical { get; }
    }
}
