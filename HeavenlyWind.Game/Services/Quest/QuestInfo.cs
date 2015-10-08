using Newtonsoft.Json.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest
{
    public class QuestInfo
    {
        public int ID { get; }

        public int Total { get; }
        public int? StartFrom { get; }

        public string ProgressRuleString { get; }

        internal QuestInfo(JToken rpJson)
        {
            ID = (int)rpJson["id"];
            Total = (int?)rpJson["total"] ?? 1;
            StartFrom = (int?)rpJson["start_from"];

            ProgressRuleString = (string)rpJson["progress_rule"];
        }
    }
}
