using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Triggers
{
    class ExpeditionTrigger : Trigger
    {
        public HashSet<int> Expeditions { get; }

        public ExpeditionTrigger(int[] rpExpeditions)
        {
            if (rpExpeditions != null)
                Expeditions = new HashSet<int>(rpExpeditions);
        }

        public override string ToString() => $"Expedition: {(Expeditions != null ? string.Join(", ", Expeditions) : "All")}";
    }
}
