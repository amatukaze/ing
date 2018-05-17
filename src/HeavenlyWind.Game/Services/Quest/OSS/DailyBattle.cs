using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.OSS
{
    [Quest(201)]
    [Quest(216)]
    [Quest(210)]
    class DailyBattle : Battle
    {
        protected override void Process(ProgressInfo rpProgress, BattleInfo rpBattle, RawBattleResult rpResult)
        {
            rpProgress.Progress++;
        }
    }
}
