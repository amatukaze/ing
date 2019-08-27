using System.Collections.Generic;
using Sakuno.ING.Game.Models.Combat;

namespace Sakuno.ING.Game.Models.Quests
{
    internal class KnownQuestTarget : QuestTarget
    {
        public KnownQuestTarget(IStatePersist statePersist, IReadOnlyList<QuestCounter> counters)
            : base(statePersist)
        {
            Counters = counters;
            foreach (var c in counters)
                c.Load(StatePersist);
        }

        public override IReadOnlyList<QuestCounter> Counters { get; }

        public void OnBattleComplete(MapRouting routing, Battle battle, BattleResult result)
        {
            foreach (var c in Counters)
                if (c is BattleResultCounter bc)
                    bc.OnBattleComplete(StatePersist, routing, battle, result);
            UpdateProgress();
        }
    }
}
