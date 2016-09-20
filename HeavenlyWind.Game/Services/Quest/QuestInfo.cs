using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.OSS;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Parsers;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest
{
    public class QuestInfo : ModelBase
    {
        public int ID { get; }

        public ExtraRewardsInfo ExtraRewards { get; }

        public int Total { get; }
        public int StartFrom { get; }
        public int DisplayTotal => Total - StartFrom;

        public bool IsDailyReset { get; }

        public int RuleVersion { get; }
        public ProgressRule[] ProgressRules { get; }

        internal QuestInfo(int rpID)
        {
            ID = rpID;
            Total = -1;

            if (rpID == 214)
            {
                OSSQuestProgressRule rOSSRule;
                if (OSSQuestProgressRule.Maps.TryGetValue(214, out rOSSRule))
                    rOSSRule.Register(this);
            }
        }
        internal QuestInfo(JToken rpJson)
        {
            ID = (int)rpJson["id"];

            var rReward = rpJson["reward"];
            if (rReward != null)
                ExtraRewards = rReward.ToObject<ExtraRewardsInfo>();

            Total = (int?)rpJson["total"] ?? 1;
            StartFrom = (int?)rpJson["start_from"] ?? 0;

            IsDailyReset = (bool?)rpJson["daily_reset"] ?? false;

            RuleVersion = (int?)rpJson["version"] ?? 1;
            if (MatchingRuleParser.Version < RuleVersion)
                return;

            var rProgressRule = (string)rpJson["progress_rule"];
            if (rProgressRule != null)
            {
                ProgressRules = MatchingRuleParser.Instance.ParseProgressRule(rProgressRule);
                foreach (var rRule in ProgressRules)
                    rRule.Register(this);
            }

            OSSQuestProgressRule rOSSRule;
            if (OSSQuestProgressRule.Maps.TryGetValue(ID, out rOSSRule))
                rOSSRule.Register(this);
        }

        public class ExtraRewardsInfo : ModelBase
        {
            [JsonProperty("materials")]
            public MaterialsReward Materials { get; internal set; }

            [JsonProperty("equipment")]
            public EquipmentReward[] Equipment { get; internal set; }

            public class MaterialsReward : ModelBase
            {
                [JsonProperty("ic")]
                public int InstantConstruction { get; internal set; }

                [JsonProperty("bucket")]
                public int Bucket { get; internal set; }

                [JsonProperty("dm")]
                public int DevelopmentMaterial { get; internal set; }

                [JsonProperty("im")]
                public int ImprovementMaterial { get; internal set; }
            }
            public class EquipmentReward : ModelBase
            {
                [JsonProperty("id")]
                public int ID { get; internal set; }

                [JsonIgnore]
                public EquipmentInfo Info => KanColleGame.Current.MasterInfo.Equipment[ID];

                [JsonProperty("count")]
                public int Count { get; internal set; }
            }
        }
    }
}
