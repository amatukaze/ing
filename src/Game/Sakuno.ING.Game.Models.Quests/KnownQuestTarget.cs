using System.Collections.Generic;
using Sakuno.ING.Game.Models.Combat;

namespace Sakuno.ING.Game.Models.Quests
{
    internal class KnownQuestTarget : QuestTarget
    {
        public KnownQuestTarget(IReadOnlyList<QuestCounter> counters)
        {
            Counters = counters;
        }

        public override IReadOnlyList<QuestCounter> Counters { get; }

        public void OnBattleComplete(MapRouting routing, Battle battle, BattleResult result)
        {
            foreach (var c in Counters)
                if (c is BattleResultCounter bc)
                    bc.OnBattleComplete(routing, battle, result);
        }
    }
}
