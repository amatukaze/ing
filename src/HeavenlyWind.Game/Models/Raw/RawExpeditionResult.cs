using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawExpeditionResult
    {
        [JsonProperty("api_ship_id")]
        public int[] Ships { get; set; }

        [JsonProperty("api_clear_result")]
        public ExpeditionResult Result { get; set; }

        [JsonProperty("api_get_exp")]
        public int AdmiralExperienceIncrement { get; set; }
        [JsonProperty("api_member_lv")]
        public int AdmiralLevel { get; set; }
        [JsonProperty("api_member_exp")]
        public int AdmiralExperience { get; set; }

        [JsonProperty("api_get_ship_exp")]
        public int[] ShipExprienceIncrement { get; set; }
        [JsonProperty("api_get_exp_lvup")]
        public int[][] ShipExperience { get; set; }

        [JsonProperty("api_maparea_name")]
        public string MapArea { get; set; }

        [JsonProperty("api_detail")]
        public string Description { get; set; }
        [JsonProperty("api_quest_name")]
        public string Name { get; set; }

        [JsonProperty("api_quest_level")]
        public int Difficulty { get; set; }

        [JsonProperty("api_get_material")]
        public JToken Materials { get; set; }

        [JsonProperty("api_useitem_flag")]
        public int[] RewardItems { get; set; }

        [JsonProperty("api_get_item1")]
        public RawItem Item1 { get; set; }
        [JsonProperty("api_get_item2")]
        public RawItem Item2 { get; set; }

        public class RawItem
        {
            [JsonProperty("api_useitem_id")]
            public int ID { get; set; }

            [JsonProperty("api_useitem_name")]
            public string Name { get; set; }

            [JsonProperty("api_useitem_count")]
            public int Count { get; set; }
        }
    }
}
