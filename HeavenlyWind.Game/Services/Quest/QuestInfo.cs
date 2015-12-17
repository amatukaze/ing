using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Parsers;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest
{
    public class QuestInfo
    {
        public int ID { get; }

        public int Total { get; }
        public int? StartFrom { get; }

        public ProgressRule[] ProgressRules { get; }

        internal QuestInfo(JToken rpJson)
        {
            ID = (int)rpJson["id"];
            Total = (int?)rpJson["total"] ?? 1;
            StartFrom = (int?)rpJson["start_from"];

            ProgressRules = MatchingRuleParser.Instance.ParseProgressRule((string)rpJson["progress_rule"]);
            foreach (var rRule in ProgressRules)
                rRule.Register(this);
        }
    }
}
