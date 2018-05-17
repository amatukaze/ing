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

        public EnemyAerialRaid EnemyAerialRaid { get; internal set; }

        internal BattleEvent(long rpTimestamp, MapInfo rpMap, RawMapExploration rpData, string rpNodeWikiID) : base(rpData)
        {
            Battle = new BattleInfo(rpTimestamp, rpData);

            int rNodeID;
            if (rpNodeWikiID.IsNullOrEmpty())
                rNodeID = rpData.Node << 16;
            else if (rpNodeWikiID.Length == 1)
                rNodeID = rpNodeWikiID[0] - 'A';
            else
            {
                rNodeID = 0;

                foreach (var c in rpNodeWikiID)
                {
                    rNodeID = (byte)c;
                    rNodeID <<= 4;
                }
            }

            EnemyEncounters = EnemyEncounterService.Instance.GetEncounters(rpMap.ID, rNodeID, rpMap.Difficulty);
        }

        long IExtraInfo.GetExtraInfo() => Battle.ID;
    }
}
