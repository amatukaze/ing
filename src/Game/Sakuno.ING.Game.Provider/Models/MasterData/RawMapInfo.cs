using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models.MasterData
{
#nullable disable
    public sealed class RawMapInfo
    {
        [JsonPropertyName("api_id")]
        public MapId Id { get; set; }
        [JsonPropertyName("api_maparea_id")]
        public MapAreaId MapAreaId { get; set; }
        [JsonPropertyName("api_no")]
        public int CategoryNo { get; set; }

        [JsonPropertyName("api_name")]
        public string Name { get; set; }
        [JsonPropertyName("api_level")]
        public int Difficulty { get; set; }
        [JsonPropertyName("api_opetext")]
        public string OperationName { get; set; }
        [JsonPropertyName("api_infotext")]
        public string Description { get; set; }

        [JsonPropertyName("api_item")]
        public UseItemId[] ItemRewards { get; set; }
        [JsonPropertyName("api_required_defeat_count")]
        public int? RequiredDefeatCount { get; set; }

        public int[] api_sally_flag { get; set; }
        public bool CanUseNormalFleet => api_sally_flag[0] != 0;
        public bool CanUseStrikingForceFleet => api_sally_flag[2] != 0;
        public bool CanUseCombinedFleet(CombinedFleetType type) =>
            (api_sally_flag[1] & (1 << ((int)type - 1))) != 0;
    }
#nullable enable
}
