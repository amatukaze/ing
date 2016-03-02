using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class BattleEvent : SortieEvent, IExtraInfo
    {
        public BattleInfo Battle { get; }

        internal BattleEvent(RawMapExploration rpData) : base(rpData)
        {
            Battle = new BattleInfo((BattleType)rpData.NodeEventSubType);
        }

        long IExtraInfo.GetExtraInfo() => Battle.ID;
    }
}
