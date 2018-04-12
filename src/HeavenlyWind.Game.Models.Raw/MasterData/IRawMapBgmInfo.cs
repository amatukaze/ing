namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public interface IRawMapBgmInfo
    {
        int Id { get; }
        int MapAreaId { get; }
        int CategoryNo { get; }

        int MapBgmId { get; }
        int NormalBattleDayBgmId { get; }
        int NormalBattleNightBgmId { get; }
        int BossBattleDayBgmId { get; }
        int BossBattleNightBgmId { get; }
    }
}
