using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Triggers;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Updaters;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Parsers
{
    using TriggerClass = Trigger;

    class MatchingRuleParser : ParserBase
    {
        public const int Version = 1;

        public static MatchingRuleParser Instance { get; } = new MatchingRuleParser();

        static Dictionary<string, ProgressRule[]> r_CachedProgressRules = new Dictionary<string, ProgressRule[]>();

        static Parser<Trigger> Trigger { get; } = from rTriggerName in Identifier
                                                  from rTrigger in TriggerClass.GetParser(rTriggerName)
                                                  select rTrigger;

        static Parser<ProgressRule> ProgressRule { get; } = from rTrigger in Trigger
                                                            from _ in Character(';').AsContinuationCondition()
                                                            select new ProgressRule(rTrigger, Updater.Default);

        MatchingRuleParser() { }

        public ProgressRule[] ParseProgressRule(string rpInput) => r_CachedProgressRules.Get(rpInput, r => Repeat(ProgressRule)(r).Value);

    }
}
