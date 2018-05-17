using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Triggers;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Parsers
{
    [TriggerName("expedition")]
    class ExpeditionTriggerParserBuilder : TriggerParserBuilder
    {
        static HybridDictionary<int[], Trigger> r_CachedFunctions = new HybridDictionary<int[], Trigger>(
            new DelegatedEqualityComparer<int[]>((x, y) => x.SequenceEqual(y), r => r.GetHashCode()));

        public override Parser<Trigger> Parser { get; } =
            from rExpeditions in Option(Repeat(from rExpeditionID in Number
                                               from _ in Character(',').AsContinuationCondition()
                                               select rExpeditionID))
            select r_CachedFunctions.Get(rExpeditions, r => new ExpeditionTrigger(r));
    }
}
