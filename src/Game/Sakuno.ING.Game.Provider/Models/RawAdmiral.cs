namespace Sakuno.ING.Game.Models
{
    public abstract class RawAdmiral : IIdentifiable
    {
        private protected RawAdmiral() { }
        public abstract int Id { get; }
        public abstract string Name { get; }
        public abstract Leveling Leveling { get; }
        public abstract AdmiralRank Rank { get; }
        public abstract string Comment { get; }
        public abstract int MaxShipCount { get; }
        public abstract int MaxEquipmentCount { get; }
        public abstract BattleStat BattleStat { get; }
        public abstract BattleStat ExerciseStat { get; }
        public abstract ExpeditionStat ExpeditionStat { get; }
        public abstract bool CanLSC { get; }
        public abstract int MaxMaterial { get; }
    }
}
