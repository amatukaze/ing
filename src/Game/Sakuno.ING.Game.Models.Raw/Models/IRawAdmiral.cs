namespace Sakuno.ING.Game.Models
{
    public interface IRawAdmiral : IIdentifiable
    {
        string Name { get; }
        Leveling Leveling { get; }
        AdmiralRank Rank { get; }
        string Comment { get; }
        int MaxShipCount { get; }
        int MaxEquipmentCount { get; }
        BattleStat BattleStat { get; }
        BattleStat PracticeStat { get; }
        ExpeditionStat ExpeditionStat { get; }
        bool CanLSC { get; }
        int MaxMaterial { get; }
    }
}
