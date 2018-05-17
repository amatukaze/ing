using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawMapMasterInfo : IID
    {
        [JsonProperty("api_id")]
        public int ID { get; set; }

        [JsonProperty("api_maparea_id")]
        public int MapAreaID { get; set; }
        [JsonProperty("api_no")]
        public int MapNo { get; set; }

        [JsonProperty("api_name")]
        public string Name { get; set; }

        [JsonProperty("api_level")]
        public int Level { get; set; }

        [JsonProperty("api_opetext")]
        public string OperationName { get; set; }
        [JsonProperty("api_infotext")]
        public string OperationInformation { get; set; }

        [JsonProperty("api_item")]
        public int[] RewardItems { get; set; }

        [JsonProperty("api_max_maphp")]
        public object MaxMapHP { get; set; }

        [JsonProperty("api_required_defeat_count")]
        public int? RequiredDefeatCount { get; set; }

        [JsonProperty("api_sally_flag")]
        public int[] SortieFleetType { get; set; }
    }
}
