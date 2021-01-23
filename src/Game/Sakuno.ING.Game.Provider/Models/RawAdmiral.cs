namespace Sakuno.ING.Game.Models
{
    public abstract class RawAdmiral : IIdentifiable
    {
        private protected RawAdmiral() { }
        public abstract int Id { get; }
        public abstract string Name { get; }
        public abstract Leveling Leveling { get; }
        public abstract AdmiralRank Rank { get; }
        public abstract int MaxShipCount { get; }
        public abstract int MaxSlotItemCount { get; }
        public abstract int MaxMaterial { get; }
    }
}
