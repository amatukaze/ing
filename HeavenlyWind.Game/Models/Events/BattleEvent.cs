using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class BattleEvent : SortieEvent, IExtraInfo
    {
        public BattleInfo Battle { get; }

        public IList<EnemyFleet> EnemyEncounters { get; }

        internal BattleEvent(MapInfo rpMap, RawMapExploration rpData, string rpNodeWikiID) : base(rpData)
        {
            Battle = new BattleInfo((BattleType)rpData.NodeEventSubType, rpData.NodeEventType == SortieEventType.BossBattle);

            int rNodeID;
            if (rpNodeWikiID.IsNullOrEmpty())
                rNodeID = rpData.Node << 16;
            else
                rNodeID = rpNodeWikiID[0] - 'A';
            EnemyEncounters = EnemyEncounterService.Instance.GetEncounters(rpMap.ID, rNodeID, rpMap.Difficulty);
        }

        long IExtraInfo.GetExtraInfo() => Battle.ID;
    }
}
