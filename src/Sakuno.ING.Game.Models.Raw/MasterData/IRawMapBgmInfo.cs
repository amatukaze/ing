namespace Sakuno.ING.Game.Models.MasterData
{
    public interface IRawMapBgmInfo
    {
        int MapBgmId { get; }
        int NormalBattleDayBgmId { get; }
        int NormalBattleNightBgmId { get; }
        int BossBattleDayBgmId { get; }
        int BossBattleNightBgmId { get; }
    }
}
